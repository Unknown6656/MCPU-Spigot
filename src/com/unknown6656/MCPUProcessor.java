package com.unknown6656;

import java.util.Stack;
import java.util.function.Consumer;


public final class MCPUProcessor
{
    private final Stack<MCPUCallframe> callstack = new Stack<>();
    private final Consumer<String> errorcallback;
    private final int[] memory = new int[4096];
    private MCPUInstruction[] instructions;
    private int instructionpointer;
    private boolean canrun;
    private int ticks;

    // C#'s event alternatives -__-
    public MCPUProcessorEvent<MCPUInstruction> OnInstructionExecuted;
    public MCPUProcessorEvent<MCPUInstruction> OnInstructionLoaded;
    public MCPUProcessorEvent<Integer> OnMemoryAccess;
    public MCPUProcessorEvent<Boolean> OnReset;
    public MCPUProcessorEvent<Void> OnStart;
    public MCPUProcessorEvent<Void> OnStop;


    public MCPUProcessor(Consumer<String> errorcallback)
    {
        this.errorcallback = errorcallback;
        
        Reset();
    }

    public synchronized void Reset()
    {
        Reset(false);
    }
    
    public synchronized void Reset(boolean resetevents)
    {
        Stop();
        
        ticks = 0;
        instructions = new MCPUInstruction[0];
        instructionpointer = -1;
        callstack.clear();
        
        ClearMemory();
        
        Raise(OnReset, resetevents);
        
        if (resetevents)
        {
            OnReset = null;
            OnStart = null;
            OnStop = null;
            OnMemoryAccess = null;
            OnInstructionLoaded = null;
            OnInstructionExecuted = null;
        }
    }
    
    public synchronized void ClearMemory()
    {
        Parallel.For(0, memory.length, i -> memory[i] = 0);
    }
    
    public synchronized boolean CanExecuteNext()
    {
        return canrun && (instructions.length > 0) && (instructionpointer < instructions.length);
    }
    
    public synchronized void ExecuteNext()
    {
        if (CanExecuteNext())
        {   
            MCPUInstruction current = instructions[instructionpointer];
            
            if (current == null)
                ExecuteNext();
            else
            {
                Raise(OnInstructionLoaded, current);
                
                MCPUCallframe frame = callstack.peek();
                
                try
                {
                    if (!current.Execute(this, frame))
                        Failwith("The execution of the instruction '" + current + "' failed.");
                    else
                        Raise(OnInstructionExecuted, current);

                    ++instructionpointer;
                }
                catch (Exception e)
                {
                    Failwith("The execution of the instruction '" + current + "' failed due to the following reason:\n" + e.getMessage());
                }
            }

            ++ticks;
        }
    }

    public int GetTicks()
    {
        return ticks;
    }
    
    public void Stop()
    {
        canrun = false;

        Raise(OnStop, null);
    }
    
    public void Start()
    {
        canrun = true;

        Raise(OnStart, null);
    }
    
    public void Load(MCPUInstruction[] instr)
    {
        Reset();
        
        instructions = instr;
        instructionpointer = 0;
        callstack.push(new MCPUCallframe());
    }
    
    private boolean CheckMemoryAddress(int addr)
    {
        boolean res;
        
        if (res = ((addr < 0) || (addr > memory.length)))
            Failwith("Invalid memory access: The address was outside the memory range. The memory address must be a value between 0 and " + memory.length + ".");
        else
            Raise(OnMemoryAccess, addr);
        
        return !res;
    }
    
    public int Memory(int addr)
    {
        return CheckMemoryAddress(addr) ? memory[addr] : 0;
    }
    
    public void Memory(int addr, int value)
    {
        if (CheckMemoryAddress(addr))
            memory[addr] = value;
    }
    
    private void Failwith(String message)
    {
        if (errorcallback != null)
            errorcallback.accept(message);

        Stop();
    }


    private <T> void Raise(MCPUProcessorEvent<T> evt, T data)
    {
        if (evt != null)
            evt.Raise(this, data);
    }
}
