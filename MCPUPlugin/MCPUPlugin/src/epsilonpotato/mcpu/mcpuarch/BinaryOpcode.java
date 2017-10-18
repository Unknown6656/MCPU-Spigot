package epsilonpotato.mcpu.mcpuarch;

import java.util.function.BiFunction;


public class BinaryOpcode extends MCPUOpcode
{
    private BiFunction<Integer, Integer, Integer> function;
    
    
    public BinaryOpcode(BiFunction<Integer, Integer, Integer> func)
    {
        function = func;
    }

    @Override
    public final int MinimumStackSize()
    {
        return 2;
    }
    
    @Override
    public final int MinimumArgumentCount()
    {
        return 0;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int y = frame.Pop();
        int x = frame.Pop();
        int res = function.apply(x, y);
        
        frame.Push(res);
    }
}
