
package epsilonpotato.mcpu.mcpuarch;


import java.util.HashMap;
import java.util.LinkedList;

import org.reflections.Reflections;

import epsilonpotato.mcpu.util.Tuple;


public abstract class MCPUOpcode
{
    static final HashMap<String, MCPUOpcode> OpcodesS = new HashMap<>();
    static final HashMap<Integer, MCPUOpcode> OpcodesN = new HashMap<>();
    
    static
    {
        // Why does java not have proper reflection? Why does it not have proper assembly metadata?
        final String pack = "epsilonpotato.mcpu.mcpuarch.opcodes";
        LinkedList<Tuple<MCPUOpcode, Integer>> opc = new LinkedList<>();
        
        try
        {
            // This will fail, because java is bitchy...
            Reflections reflections = new Reflections(pack);
            
            for (Class<? extends MCPUOpcode> stype : reflections.getSubTypesOf(MCPUOpcode.class))
                try
                {
                    tryAdd(opc, stype);
                }
                catch (Exception e)
                {
                }
        }
        catch (NoClassDefFoundError e)
        {
            // We have to do this shit manually because java does not have reflection.
            // and NO, your fucking package 'java.lang.shit.reflection' IS NOT FUCKING REFLECTION !!! THATS BINARY SHITTING.
            // I'm hardcoding a list here for now ...... fucking java
            // Biggest piece of shit I ever had to write ....... (well, I let an automated tool write)
            final String instr = "Abk Abs Add And Br Brfalse Brtrue Call Clr Decr Div Dup Eq Exp Geq Gt Halt Incr Ldarg " +
                                 "Ldc Ldio Ldiosz Ldglob Ldloc Ldmem Ldmemsz Leq Log Log10 Log2 Loge Lt Mod Mov Mul Neg " +
                                 "Neq Nop Not Or Pow Pop Regloc Regglob Ret Rol Ror Shl Shr Sign Starg Stio Stiodir Stloc " +
                                 "Stmem Stglob Sub Swap Syscall Xor ";
            // Dear Microsoft,
            // Please buy Oracle company (or maybe don't, because your stock market value will crash)
            // and make the usage and development of/with Java illegal. (or at least ship a virus with the next jre-release)
            // Thank you.
            
            for (String name : instr.split(" "))
                try
                {
                    tryAdd(opc, Class.forName(pack + '.' + name));
                }
                catch (Exception x)
                {
                }
        }
        
        for (Tuple<MCPUOpcode, Integer> t : opc)
        {
            OpcodesS.put(t.x.toString(), t.x);
            OpcodesN.put(t.y, t.x);
        }
    }
    
    
    public int MinimumArgumentCount()
    {
        return 0;
    }
    
    public int MinimumStackSize()
    {
        return -1;
    }
    
    public abstract void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc);
    
    @Override
    public String toString()
    {
        return getClass().getSimpleName().toLowerCase();
    }

    
    public int getNumber()
    {
        return this.getClass().getAnnotation(MCPUOpcodeNumber.class).value();
    }

    private static boolean tryAdd(LinkedList<Tuple<MCPUOpcode, Integer>> opc, Class<?> type)
    {
        try
        {
            MCPUOpcode opcode = (MCPUOpcode)type.newInstance();
            MCPUOpcodeNumber num = opcode.getClass().getAnnotation(MCPUOpcodeNumber.class);
            
            opc.add(new Tuple<>(opcode, (int)num.value()));
            
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    public static MCPUOpcode get(int opc)
    {
        return OpcodesN.get(opc);
    }
}
