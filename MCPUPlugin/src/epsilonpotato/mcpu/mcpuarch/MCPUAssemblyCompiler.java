
package epsilonpotato.mcpu.mcpuarch;


import java.util.HashMap;
import java.util.LinkedList;
import java.util.logging.Level;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.bukkit.ChatColor;

import epsilonpotato.mcpu.util.Parallel;
import epsilonpotato.mcpu.util.Tuple;


public final class MCPUAssemblyCompiler
{
    public static MCPUInstruction[] fromBytes(byte[] raw)
    {
        int[] arr = new int[raw.length / 4];
        
        Parallel.For(0, arr.length, i -> arr[i] = (raw[i] << 24) | (raw[i + 1] << 16) | (raw[i + 2] << 8) | raw[i + 3]);
        
        MCPUInstruction[] ins = new MCPUInstruction[arr[0]];
        int offs = 1;
        
        for (int i = 0; i < ins.length; ++i)
        {
            int opc = arr[offs++];
            int len = arr[offs++];
            int[] args = new int[len];
            
            for (int j = 0; j < len; ++j)
                args[j] = arr[offs++];
            
            ins[i] = new MCPUInstruction(MCPUOpcode.get(opc), args);
        }
        
        return ins;
    }
    
    public static MCPUCompilerResult Compile(String code)
    {
        if (code == null)
            code = "";

        return Compile(code.replaceAll("\\r\\n", "\n").replaceAll("\\n\\r", "\n").trim().split("[\\r\\n]"));
    }
    
    public static MCPUCompilerResult Compile(String[] lines)
    {
        final Tuple<String[], HashMap<Integer, Integer>> cleaned = CleanLines(lines);
        final LinkedList<PrecompiledInstruction> preinstr = new LinkedList<>();
        final HashMap<String, Integer> unresolved = new HashMap<>();
        final HashMap<String, Integer> labels = new HashMap<>();
        final LinkedList<String> errors = new LinkedList<>();
        final Pattern pline = Pattern.compile("(?<instr>[a-z]\\w*)((?<args>(\\s+([0-9]+|0?x[0-9a-f]+|true|false|[a-z_]\\w*))*)|\\\"(?<strarg>[^\\\"]*)\\\")");
        Matcher match;
        
        lines = cleaned.x;
        
        for (int linenr = 0; linenr < lines.length; ++linenr)
        {
            String line = lines[linenr];

            line = ChatColor.stripColor(line);
            
            if (line.contains(";"))
                line = line.substring(line.indexOf(';') + 1).trim();
            
            if (line.matches("^[a-z_]\\w*\\:"))
            {
                line = line.substring(0, line.indexOf(':')).toLowerCase();
                
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
                match = pline.matcher(line);
                
                if (match.find())
                {
                    String instr = match.group("instr");
                    
                    if (instr == null || instr == "null")
                        instr = "nop";

                    if (!MCPUOpcode.OpcodesS.containsKey(instr.toLowerCase()))
                        Error(cleaned.y, errors, linenr, "The instruction '" + instr + "' could not be found.");
                    else
                    {
                        MCPUOpcode opcode = MCPUOpcode.OpcodesS.get(instr);
                        PrecompiledInstruction ins = new PrecompiledInstruction(opcode);
                        String strarg = match.group("strarg");
                        String[] rargs = null;
                        int i = 0;
                        
                        if (strarg != null)
                        {
                            byte[] bytes = strarg.getBytes();
                            int[] iargs = new int[(int)Math.ceil(bytes.length / 4.0)];
                            
                            Parallel.For(0, iargs.length, j -> iargs[j] = (bytes[j] << 24) |
                                                                          (j + 1 < iargs.length ? (bytes[j + 1] << 16) : 0) |
                                                                          (j + 2 < iargs.length ? (bytes[j + 2] << 8) : 0) |
                                                                          (j + 3 < iargs.length ? bytes[j + 3] : 0));

                            for (int arg : iargs)
                                ins.AddArgument(arg, PrecompiledArgumentType.RAW);
                            
                            preinstr.add(ins);
                        }
                        else
                        {
                            rargs = match.group("args").trim().split("\\s+");

                            if (opcode.MinimumArgumentCount() > rargs.length)
                                Error(cleaned.y, errors, linenr, "The instruction '" + instr + "' requires at least " + opcode.MinimumArgumentCount() + " arguments.");
                            else
                            {
                                for (String arg : rargs)
                                    if (arg != null)
                                        try
                                        {
                                            arg = arg.trim().toLowerCase();
                                            
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
