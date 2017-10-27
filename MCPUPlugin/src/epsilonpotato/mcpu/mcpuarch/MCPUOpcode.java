
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
        final LinkedList<Tuple<MCPUOpcode, Integer>> opc = new LinkedList<>();
        final String pack = "epsilonpotato.mcpu.mcpuarch.opcodes";
        final Reflections reflections = new Reflections(pack);
        
        for (Class<? extends MCPUOpcode> stype : reflections.getSubTypesOf(MCPUOpcode.class))
            try
            {
                tryAdd(opc, stype);
            }
            catch (Exception e)
            {
            }
        
        for (Tuple<MCPUOpcode, Integer> t : opc)
        {
            OpcodesS.put(t.x.getName(), t.x);
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
    public final String toString()
    {
        return String.format("%d (%04xh) - %s", getNumber(), getNumber(), getName());
    }

    public final String getName()
    {
        return getClass().getSimpleName().toLowerCase();
    }
    
    public final int getNumber()
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
