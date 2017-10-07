package com.unknown6656.opcodes;

import java.util.function.Function;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


class UnaryOpcode extends MCPUOpcode
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
    public final boolean Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int x = frame.Pop();
        int res = function.apply(x);
        
        frame.Push(res);
        
        return true;
    }
}
