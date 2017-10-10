package com.unknown6656;

import java.util.function.Function;


public class UnaryOpcode extends MCPUOpcode
{
    private Function<Integer, Integer> function;
    
    
    public UnaryOpcode(Function<Integer, Integer> func)
    {
        function = func;
    }

    @Override
    public final int MinimumStackSize()
    {
        return 1;
    }
    
    @Override
    public final int MinimumArgumentCount()
    {
        return 0;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int x = frame.Pop();
        int res = function.apply(x);
        
        frame.Push(res);
    }
}
