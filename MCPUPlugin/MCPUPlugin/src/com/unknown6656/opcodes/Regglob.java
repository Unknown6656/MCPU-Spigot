package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


public final class Regglob extends MCPUOpcode
{
    @Override
    public int MinimumStackSize()
    {
        return 0;
    }
    
    @Override
    public int MinimumArgumentCount()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int oldsz = proc.globalscount;
        int newsz = proc.globalscount = arguments[0];
        int memlen = proc.MemorySize();
        
        if (oldsz < newsz)
            for (int i = oldsz; i < newsz; ++i)
                proc.Globals(i, 0);
        else
            for (int i = newsz; i < oldsz; ++i)
                proc.Memory(memlen - i, 0);
    }
}
