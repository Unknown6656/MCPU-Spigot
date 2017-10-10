package com.unknown6656.opcodes;

import java.util.function.BiFunction;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


class BinaryOpcode extends MCPUOpcode
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
