package com.unknown6656;

import java.util.HashMap;
import java.util.LinkedList;

import org.reflections.Reflections;


public abstract class MCPUOpcode
{
    static final HashMap<String, MCPUOpcode> Opcodes = new HashMap<>();
    
    static
    {
        // Why does java not have proper reflection? Why does it not have proper assembly metadata?
        final String pack = "com.unknown6656.opcodes";
        LinkedList<MCPUOpcode> opc = new LinkedList<>();
        
        try
        {
            Reflections reflections = new Reflections(pack);
            
            for (Class<? extends MCPUOpcode> stype : reflections.getSubTypesOf(MCPUOpcode.class))
                try
                {
                    opc.add(stype.newInstance());
                }
                catch (Exception e)
                {
                }
        }
        catch (NoClassDefFoundError e)
        {
            // We have to do this shit manually
            // I'm hardcoding a list here for now ...... fucking java
            // Biggest piece of shit I ever had to write .......
            final String instr = "Add And Br Brtrue Brfalse Call Div Decr Halt Incr Ldarg Ldc Ldmem Mod Mov Mul Neg Not Nop Or Ret Starg Stmem Sub Swap Xor";
            // Dear Microsoft,
            // Please buy Oracle company (or maybe don't, because your stock market value will crash)
            // and make the usage and development of/with Java illegal.
            // Thank you.
            
            for (String name : instr.split(" "))
                try
                {
                    Class<?> type = Class.forName(pack + '.' + name);
                    
                    opc.add((MCPUOpcode)type.newInstance());
                }
                catch (Exception x)
                {
                }
        }
        
        for (MCPUOpcode c : opc)
            Opcodes.put(c.toString(), c);
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
}
