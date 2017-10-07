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
            // I'm hardcoding an array here for now...... fucking java
            for (String name : "Add And Br Brtrue Brfalse Div Halt Ldarg Ldc Ldmem Mod Mov Mul Neg Not Nop Or Starg Stmem Sub Swap Xor".split(" "))
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
    
    public abstract boolean Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc);
    
    @Override
    public String toString()
    {
        return getClass().getSimpleName().toLowerCase();
    }
}
