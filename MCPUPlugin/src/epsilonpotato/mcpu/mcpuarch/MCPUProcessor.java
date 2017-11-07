package epsilonpotato.mcpu.mcpuarch;


import java.util.List;
import java.util.Arrays;
import java.util.Stack;

import org.bukkit.entity.Player;

import epsilonpotato.mcpu.core.EmulatedProcessorEvent;
import epsilonpotato.mcpu.core.MCPUCore;
import epsilonpotato.mcpu.core.SquareEmulatedProcessor;
import epsilonpotato.mcpu.util.Parallel;
import epsilonpotato.mcpu.util.YamlConfiguration;

import org.bukkit.ChatColor;
import org.bukkit.Location;
import org.bukkit.World;


public final class MCPUProcessor extends SquareEmulatedProcessor
{
    private static final long serialVersionUID = 9156823759839023027L;
    private Stack<MCPUCallframe> callstack = new Stack<>();
    private transient MCPUInstruction[] instructions;
    private int[] memory = new int[1024];
    public int globalscount;
    
    public int InstructionPointer;
    
    public EmulatedProcessorEvent<MCPUInstruction> onInstructionExecuted;
    public EmulatedProcessorEvent<MCPUInstruction> onInstructionLoaded;
    public EmulatedProcessorEvent<Integer> onMemoryAccess;
    public EmulatedProcessorEvent<Boolean> onReset;
    public EmulatedProcessorEvent<Void> onStart;
    public EmulatedProcessorEvent<Void> onStop;
    
    
    public MCPUProcessor(Player creator, World world, int x, int y, int z, int count) throws Exception
    {
        super(creator, new Location(world, x, y, z), count);
        
        reset();
    }
    
    public int getMemorySize()
    {
        return memory == null ? -1 : memory.length;
    }
    
    private boolean CheckMemoryAddress(int addr)
    {
        boolean res;
        
        if (res = ((addr < 0) || (addr > memory.length - globalscount)))
            Failwith("Invalid memory access: The address was outside the memory range. The memory address must be a value between 0 and " + (memory.length - globalscount - 1) + ".");
        else
            raise(onMemoryAccess, addr);
        
        return !res;
    }
    
    public void clearMemory()
    {
        if (memory == null)
            memory = new int[1024];
        else
            Parallel.For(0, memory.length, i -> memory[i] = 0);
    }
    
    public int getMemory(int addr)
    {
        return CheckMemoryAddress(addr) ? memory[addr] : 0;
    }
    
    public void setMemory(int addr, int value)
    {
        if (CheckMemoryAddress(addr))
            memory[addr] = value;
    }
    
    public int getGlobal(int addr)
    {
        if ((addr >= globalscount) || (addr < 0))
        {
            Failwith("Invalid global variable access: The address was outside the memory range. The global variable address must be a value between 0 and " + (globalscount - 1) + ".");
            
            return 0;
        }
        else
            return memory[memory.length - addr];
    }
    
    public void setGlobal(int addr, int value)
    {
        if ((addr >= globalscount) || (addr < 0))
            Failwith("Invalid global variable access: The address was outside the memory range. The global variable address must be a value between 0 and " + (globalscount - 1) + ".");
        else
            memory[memory.length - addr] = value;
    }
    
    private void setTicks(long i)
    {
        ticks = i;
        
        getSign(s -> s.setLine(3, "T: " + ticks));
    }
    
    @Override
    public void executeNextInstruction()
    {
        if ((instructions.length > 0) && (InstructionPointer < instructions.length))
        {
            MCPUInstruction current = instructions[InstructionPointer];
            
            if (current == null)
                executeNextInstruction();
            else
            {
                raise(onInstructionLoaded, current);
                
                MCPUCallframe frame = callstack.peek();
                MCPUOpcode opcode = current.getOPCode();
                
                getSign(s -> s.setLine(2, opcode.toString()));
                
                try
                {
                    if ((opcode.MinimumStackSize() >= 0) && (opcode.MinimumArgumentCount() > frame.StackSize()))
                        Failwith("The execution of the opcode '" + opcode + "' requires at least " + opcode.MinimumArgumentCount() + " values to be on the current stack.");
                    else
                        current.Execute(this, frame);
                    
                    raise(onInstructionExecuted, current);
                    
                    ++InstructionPointer;
                }
                catch (Exception e)
                {
                    Failwith("The execution of the instruction '" + current + "' failed due to the following reason:\n" + e.getMessage());
                }
                
                if (InstructionPointer < instructions.length)
                {
                    current = instructions[InstructionPointer];
                    
                    if (current != null)
                    {
                        MCPUOpcode opc = current.getOPCode();
                        
                        getSign(s -> s.setLine(2, opc.toString()));
                    }
                }
            }
            
            setTicks(ticks + 1);
        }
    }
    
    @Override
    protected void innerReset()
    {
        reset(false);
    }
    
    @Override
    protected void innerStop()
    {
        raise(onStop, null);
    }
    
    @Override
    protected void innerStart()
    {
        raise(onStart, null);
    }
    
    public void reset(boolean resetevents)
    {
        stop();
        
        instructions = new MCPUInstruction[0];
        InstructionPointer = -1;
        
        if (callstack == null)
            callstack = new Stack<>();
        else
            callstack.clear();
        
        getSign(s -> s.setLine(2, ""));
        setTicks(0);
        clearMemory();
        
        raise(onReset, resetevents);
        
        if (resetevents)
        {
            onReset = null;
            onStart = null;
            onStop = null;
            onMemoryAccess = null;
            onInstructionLoaded = null;
            onInstructionExecuted = null;
        }
    }
    
    public void restart()
    {
        load(instructions);
    }
    
    @Override
    public final boolean load(String code)
    {
        MCPUCompilerResult res = MCPUAssemblyCompiler.Compile(code);
        
        if (res.Success)
            load(res.Instructions);
        else
            MCPUCore.print(creator, ChatColor.RED, res.ErrorMessage);
        
        return res.Success;
    }
    
    public void load(MCPUInstruction[] instr)
    {
        reset();
        
        if (instructions != null)
        {
            instructions = instr;
            InstructionPointer = 0;
            
            PushCall(new MCPUCallframe());
            
            getSign(s -> s.setLine(2, InstructionPointer < instructions.length ? instructions[InstructionPointer].getOPCode().toString() : "---"));
        }
    }
    
    public String AssemblyInstructions()
    {
        StringBuilder sb = new StringBuilder();
        
        if (instructions != null)
            for (MCPUInstruction ins : instructions)
                sb.append(ins + "\n");
            
        return sb.toString().trim();
    }
    
    public MCPUCallframe PopCall()
    {
        return callstack.pop();
    }
    
    public MCPUCallframe PeekCall()
    {
        return callstack.peek();
    }
    
    public void PushCall(MCPUCallframe frame)
    {
        callstack.push(frame);
    }
    
    private <T> void raise(EmulatedProcessorEvent<T> evt, T data)
    {
        if (evt != null)
            evt.Raise(this, data);
    }
    
    private void Failwith(String message)
    {
        if (onError != null)
            onError.Raise(this, message);
        
        stop();
    }
    
    public String getState()
    {
        return String.format("(%d|%d|%d) created by %s (%s), %s, %s", x, y, z, creator.getDisplayName(), creator.getAddress().toString(), canrun ? "running" : "halted", isEnabled() ? "enabled" :
                                                                                                                                                                                     "disabled");
    }
    
    public Location getCenterLocation()
    {
        return new Location(world, x + (xsize - 1) / 2, y, z + (zsize - 1) / 2);
    }
    

    @Override
    protected final void serializeProcessorState(YamlConfiguration conf)
    {
        YamlConfiguration confMemory = conf.getOrCreateSection("memory");
        YamlConfiguration confCallstack = conf.getOrCreateSection("callstack");
        YamlConfiguration confInstructions = conf.getOrCreateSection("instructions");
        
        confMemory.set("count", memory.length);
        confMemory.set("data", Arrays.asList(memory));
        
        conf.set("globals", globalscount);
        
        int num = 0;
        
        if (instructions != null)
            for (MCPUInstruction ins : instructions)
            {
                confInstructions.set("ins_" + num + ".opc", ins.getOPCode().getNumber());
                confInstructions.set("ins_" + num + ".args", Arrays.asList(ins.getArguments()));
                
                ++num;
            }

        confInstructions.set("__ptr", InstructionPointer);
        confInstructions.set("count", num);
        
        confCallstack.set("size", callstack.size());

        num = 0;
        
        for (MCPUCallframe frame : callstack)
        {
            YamlConfiguration confCallframe = confCallstack.getOrCreateSection("frame_" + num);
            YamlConfiguration confStack = confCallframe.getOrCreateSection("stack");
            
            confCallframe.set("locals", Arrays.asList(frame.Locals));
            confCallframe.set("args", Arrays.asList(frame.Arguments));
            confStack.set("size", frame.StackSize());

            int stacknum = 0;
            
            for (int v : frame.Stack)
            {
                confStack.set("value_" + stacknum, v);
             
                ++stacknum;
            }

            ++num;
        }
    }
    
    @Override
    protected final void deserializeProcessorState(YamlConfiguration conf)
    {
        YamlConfiguration confMemory = conf.getOrCreateSection("memory");
        YamlConfiguration confCallstack = conf.getOrCreateSection("callstack");
        YamlConfiguration confInstructions = conf.getOrCreateSection("instructions");

        memory = new int[confMemory.getInt("count", 0)];

        int i = 0;
        
        for (int val : confMemory.getList("data", Integer.class))
            if (i < memory.length)
                memory[i++] = val;
            else
                break;
        
        globalscount = conf.getInt("globals", 0);
        InstructionPointer = confInstructions.getInt("__ptr", 0);
        
        int num = confInstructions.getInt("count", 0);
        
        instructions = new MCPUInstruction[num];
        
        for (i = 0; i < num; ++i)
        {
            int opc = confInstructions.getInt("ins_" + num + ".opc", 0);
            List<Integer> arglist = confInstructions.getList("ins_" + num + ".args", Integer.class); 
            int[] args = new int[arglist.size()];
            int cnt = 0;
            
            for (int val : arglist)
                args[cnt++] = val;
            
            instructions[i] = new MCPUInstruction(MCPUOpcode.get(opc), args);
        }

        callstack.clear();
        
        num = confCallstack.getInt("size", 0);
        
        for (i = 0; i < num; ++i)
        {
            YamlConfiguration confCallframe = confCallstack.getOrCreateSection("frame_" + i);
            YamlConfiguration confStack = confCallframe.getOrCreateSection("stack");
            
            List<Integer> locals = confCallframe.getList("locals", Integer.class);
            List<Integer> arguments = confCallframe.getList("args", Integer.class);
            int[] stack = new int[confStack.getInt("size", 0)];
            int[] args = new int[arguments.size()];
            int[] loc = new int[locals.size()];
            int stacknum = 0;
            int index;
            
            for (index = 0; index < stacknum; ++index)
                stack[index] = confStack.getInt("value_" + stacknum, 0);

            index = 0;
            
            for (int v : locals)
                loc[index++] = v;

            index = 0;
            
            for (int v : arguments)
                args[index++] = v;
            
            callstack.push(new MCPUCallframe(args, loc, stack));
        }
    }
}
