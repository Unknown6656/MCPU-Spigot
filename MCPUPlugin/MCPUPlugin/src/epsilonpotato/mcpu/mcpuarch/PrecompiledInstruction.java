package epsilonpotato.mcpu.mcpuarch;

import java.util.LinkedList;

import epsilonpotato.mcpu.core.Tuple;


public final class PrecompiledInstruction
{
    public LinkedList<Tuple<Integer, PrecompiledArgumentType>> Arguments = new LinkedList<>();
    public MCPUOpcode Opcode;
    
    
    public PrecompiledInstruction(MCPUOpcode opc)
    {
        Opcode = opc;
    }
    
    public void AddArgument(int val, PrecompiledArgumentType type)
    {
        Arguments.add(new Tuple<Integer, PrecompiledArgumentType>(val, type));
    }
    
    @Override
    public String toString()
    {
        StringBuilder sb = new StringBuilder();
        
        sb.append(Opcode.toString());
        
        for (Tuple<Integer, PrecompiledArgumentType> arg : Arguments)
            sb.append(' ').append(arg.y == PrecompiledArgumentType.LABEL ? "@" : arg.y == PrecompiledArgumentType.UNRESOLVED ? "?" : "").append(arg.x);
        
        return sb.toString();
    }
}
