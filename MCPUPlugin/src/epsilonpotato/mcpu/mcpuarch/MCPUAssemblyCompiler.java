package epsilonpotato.mcpu.mcpuarch;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.logging.Level;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import epsilonpotato.mcpu.util.Tuple;


public final class MCPUAssemblyCompiler
{
    public static MCPUCompilerResult Compile(String code)
    {
        if (code == null)
            code = "";
        
        return Compile(code.replaceAll("\\r\\n", "\n").replaceAll("\\n\\r", "\n").split("[\\r\\n]"));
    }
    
    public static MCPUCompilerResult Compile(String[] lines)
    {
        final Tuple<String[], HashMap<Integer, Integer>> cleaned = CleanLines(lines);
        final LinkedList<PrecompiledInstruction> preinstr = new LinkedList<>();
        final HashMap<String, Integer> unresolved = new HashMap<>();
        final HashMap<String, Integer> labels = new HashMap<>();
        final LinkedList<String> errors = new LinkedList<>();
        
        lines = cleaned.x;
        
        for (int linenr = 0; linenr < lines.length; ++linenr)
        {
            String line = lines[linenr];
            Pattern pmcesc = Pattern.compile("�[0-9a-fklmnor]");
            Matcher match = pmcesc.matcher(line);
            
            line = match.replaceAll("");
            
            if (line.matches("^[a-z_]\\w*\\:"))
            {
                line = line.substring(0, line.indexOf(':'));
                
                if (labels.containsKey(line))
                    Error(cleaned.y, errors, linenr, "The label/function '" + line + "' does already exist.");
                else if (unresolved.containsKey(line))
                {
                    labels.put(line, linenr);
                    
                    int oldlnr = unresolved.remove(line);
                    
                    for (PrecompiledInstruction pi : preinstr)
                    {
                        LinkedList<Tuple<Integer, PrecompiledArgumentType>> newargs = new LinkedList<>();
                        
                        for (Tuple<Integer, PrecompiledArgumentType> t : pi.Arguments)
                            if ((t.x == oldlnr) && (t.y == PrecompiledArgumentType.UNRESOLVED))
                                newargs.add(new Tuple<Integer, PrecompiledArgumentType>(linenr, PrecompiledArgumentType.LABEL));
                            else
                                newargs.add(t);
                            
                        pi.Arguments = newargs;
                    }
                }
                else
                    labels.put(line, linenr);
                
                preinstr.add(new PrecompiledInstruction(MCPUOpcode.OpcodesS.get("nop")));
            }
            else
            {
                Pattern pline = Pattern.compile("(?<instr>[a-z]\\w*)(?<args>(\\s+([0-9]+|0?x[0-9a-f]+|true|false|[a-z_]\\w*))*)");
                
                match = pline.matcher(line);
                
                if (match.matches())
                {
                    String instr = match.group("instr");
                    String[] rargs = match.group("args").trim().split("\\s+");
                    
                    if (!MCPUOpcode.OpcodesS.containsKey(instr))
                        Error(cleaned.y, errors, linenr, "The instruction '" + instr + "' could not be found.");
                    else
                    {
                        MCPUOpcode opcode = MCPUOpcode.OpcodesS.get(instr);
                        int i = 0;
                        
                        if (opcode.MinimumArgumentCount() > rargs.length)
                            Error(cleaned.y, errors, linenr, "The instruction '" + instr + "' requires at least " + opcode.MinimumArgumentCount() +
                                                             " arguments.");
                        else
                        {
                            PrecompiledInstruction ins = new PrecompiledInstruction(opcode);
                            
                            for (String arg : rargs)
                                try
                                {
                                    arg = arg.trim();
                                    
                                    if (arg.length() > 0)
                                    {
                                        if (arg.matches("[0-9]+"))
                                            ins.AddArgument(Integer.parseInt(arg), PrecompiledArgumentType.RAW);
                                        else if (arg.matches("0?x[0-9a-f]+"))
                                            ins.AddArgument(Integer.parseInt(arg.substring(arg.indexOf('x') + 1), 16), PrecompiledArgumentType.RAW);
                                        else if (arg.matches("(true|false)"))
                                            ins.AddArgument(Boolean.parseBoolean(arg) ? 1 : 0, PrecompiledArgumentType.RAW);
                                        else if (!labels.containsKey(arg))
                                        {
                                            unresolved.put(arg, linenr);
                                            
                                            ins.AddArgument(linenr, PrecompiledArgumentType.UNRESOLVED);
                                        }
                                        else
                                            ins.AddArgument(labels.get(arg), PrecompiledArgumentType.LABEL);
                                        
                                        ++i;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Error(cleaned.y, errors, linenr, "The argument no. " + (i + 1) + " ('" + arg + "') could not be parsed.");
                                }
                            
                            preinstr.add(ins);
                        }
                    }
                }
                else if (line.trim().length() > 0)
                    Error(cleaned.y, errors, linenr, "The expression '" + line + "' could not be parsed.");
                else
                    preinstr.add(new PrecompiledInstruction(MCPUOpcode.OpcodesS.get("nop")));
            }
        }
        
        for (String s : unresolved.keySet())
            Error(cleaned.y, errors, unresolved.get(s), "The label/function '" + s + "' could not be resolved.");
        
        // only temporary
        for (PrecompiledInstruction pi : preinstr)
            MCPUPlugin.log.log(Level.INFO, pi.toString());
        
        return errors.size() > 0 ? new MCPUCompilerResult(null, String.join("\n", errors)) : new MCPUCompilerResult(Optimize(cleaned.y, preinstr), null);
    }
    
    private static MCPUInstruction[] Optimize(HashMap<Integer, Integer> mapper, LinkedList<PrecompiledInstruction> preinstr)
    {
        MCPUInstruction[] compiled = new MCPUInstruction[preinstr.size()];
        int i = 0;
        
        for (PrecompiledInstruction ins : preinstr)
        {
            int[] args = new int[ins.Arguments.size()];
            int j = 0;
            
            for (Tuple<Integer, PrecompiledArgumentType> arg : ins.Arguments)
                args[j++] = arg.x;
            
            compiled[i++] = new MCPUInstruction(ins.Opcode, args);
        }
        
        return compiled;
    }
    
    private static void Error(HashMap<Integer, Integer> mapper, LinkedList<String> errors, int line, String msg)
    {
        if (mapper.containsKey(line))
            line = mapper.get(line);
        
        errors.add("l." + (line + 1) + ": " + msg);
    }
    
    private static Tuple<String[], HashMap<Integer, Integer>> CleanLines(String[] lines)
    {
        HashMap<Integer, Integer> mapper = new HashMap<Integer, Integer>();
        
        if (lines == null)
            lines = new String[] { "" };
        else
        {
            LinkedList<String> cleaned = new LinkedList<>();
            
            for (int linenr = 0, local = 0; linenr < lines.length; ++linenr)
            {
                String line = lines[linenr].toLowerCase();
                
                if (line.contains("//"))
                    line = line.substring(0, line.indexOf("//"));
                
                mapper.put(local, linenr);
                
                if (!(line = line.trim()).isEmpty())
                {
                    cleaned.add(line);
                    
                    ++local;
                }
            }
        }
        
        return new Tuple<String[], HashMap<Integer, Integer>>(lines, mapper);
    }
}
