package epsilonpotato.mcpu.mcpuarch;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.URI;
import java.net.URL;
import java.util.ArrayList;
import java.util.Stack;
import java.util.function.Function;

import org.bukkit.entity.Player;

import epsilonpotato.mcpu.core.EmulatedProcessor;
import epsilonpotato.mcpu.core.EmulatedProcessorEvent;
import epsilonpotato.mcpu.core.Parallel;
import epsilonpotato.mcpu.core.Triplet;
import epsilonpotato.mcpu.core.Tuple;

import org.bukkit.Location;
import org.bukkit.Material;
import org.bukkit.World;
import org.bukkit.block.Block;


public final class MCPUProcessor extends EmulatedProcessor
{
    private final Stack<MCPUCallframe> callstack = new Stack<>();
    private final ArrayList<Tuple<Byte, Boolean>> ioports; // number, direction (true=out, false=in)
    private final int[] memory = new int[4096];
    private MCPUInstruction[] instructions;
    private final int sidecount;
    public int globalscount;
    private boolean canrun;
    private long ticks;
    
    public int InstructionPointer;
    private Triplet<Integer, Integer, Integer> Location;
    
    public EmulatedProcessorEvent<MCPUInstruction> onInstructionExecuted;
    public EmulatedProcessorEvent<MCPUInstruction> onInstructionLoaded;
    public EmulatedProcessorEvent<Integer> onMemoryAccess;
    public EmulatedProcessorEvent<Boolean> onReset;
    public EmulatedProcessorEvent<Void> onStart;
    public EmulatedProcessorEvent<Void> onStop;
    
    
    public MCPUProcessor(Player creator, World world, int x, int y, int z, int xs, int ys, int zs, int count)
    {
        super(creator, new Location(world, x, y, z), new Triplet<>(xs, ys, zs), count * 4);
        
        this.ioports = new ArrayList<>(4 * count);
        this.sidecount = count;
        
        for (int i = 0; i < 4 * count; ++i)
            this.ioports.add(new Tuple<Byte, Boolean>((byte)0, false));
        
        reset();
    }
    
    public void reset()
    {
        reset(false);
    }
    
    public void reset(boolean resetevents)
    {
        stop();
        
        instructions = new MCPUInstruction[0];
        InstructionPointer = -1;
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
    
    public void clearMemory()
    {
        Parallel.For(0, memory.length, i -> memory[i] = 0);
    }
    
    public boolean canExecuteNext()
    {
        return canrun && IsEnabled() && (instructions.length > 0) && (InstructionPointer < instructions.length);
    }
    
    public void nextInstruction()
    {
        if (canExecuteNext())
        {
            MCPUInstruction current = instructions[InstructionPointer];
            
            if (current == null)
                nextInstruction();
            else
            {
                raise(onInstructionLoaded, current);
                
                MCPUCallframe frame = callstack.peek();
                MCPUOpcode opcode = current.GetOPCode();
                
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
                        MCPUOpcode opc = current.GetOPCode();
                        
                        getSign(s -> s.setLine(2, opc.toString()));
                    }
                }
            }
            
            setTicks(ticks + 1);
        }
    }
    
    private void setTicks(long i)
    {
        ticks = i;
        
        getSign(s -> s.setLine(3, "T: " + ticks));
    }
    
    public Player getCreator()
    {
        return creator;
    }
    
    public long getTicksElapsed()
    {
        return ticks;
    }
    
    public void setIODirection(int port, boolean direction)
    {
        CheckIOExists(port, p -> p.y = direction);
    }
    
    public boolean getIODirection(int port)
    {
        return CheckIOExists(port, p -> p.y);
    }
    
    public void setIO(int port, byte value)
    {
        CheckIOExists(port, p ->
        {
            if (p.y)
                ioports.get(port).x = value;
            else
                Failwith("The port no. " + port + " is not set for input.");
            
            return null;
        });
    }
    
    public byte getIO(int port)
    {
        return (byte)CheckIOExists(port, p ->
        {
            if (!p.y)
                return ioports.get(port).x;
            else
                Failwith("The port no. " + port + " is not set for output.");

            return 0;
        });
    }
    
    public int Globals(int addr)
    {
        if ((addr >= globalscount) || (addr < 0))
        {
            Failwith("Invalid global variable access: The address was outside the memory range. The global variable address must be a value between 0 and " + (globalscount - 1) + ".");
            
            return 0;
        }
        else
            return memory[memory.length - addr];
    }
    
    public void Globals(int addr, int value)
    {
        if ((addr >= globalscount) || (addr < 0))
            Failwith("Invalid global variable access: The address was outside the memory range. The global variable address must be a value between 0 and " + (globalscount - 1) + ".");
        else
            memory[memory.length - addr] = value;
    }

    private <T> T CheckIOExists(int port, Function<Tuple<Byte, Boolean>, T> callback)
    {
        if ((port < 0) || (port >= ioports.size()))
        {
            Failwith("The I/O port " + port + " does not exist. The port number must be a valid number between [0," + (ioports.size() - 1) + "].");
            
            return null;
        }
        else
            return callback.apply(ioports.get(port));
    }
    
    public void stop()
    {
        canrun = false;
        
        raise(onStop, null);
    }
    
    public void start()
    {
        canrun = true;
        
        raise(onStart, null);
    }
    
    public void restart()
    {
        load(instructions);
    }
    
    @Override
    public boolean load(URI uri)
    {
        StringBuilder code = new StringBuilder();

        try
        {
            String s = null;
            BufferedReader reader = new BufferedReader(new InputStreamReader(new URL(uri.toString()).openStream()));
            
            while ((s = reader.readLine()) != null)
                code.append('\n').append(s);
        }
        catch (Exception e1)
        {
            if (uri.getScheme().toLowerCase().equals("raw"))
                code.append(uri.getPath());
            else
                // dunno ? 
                return false;
        }
        
        MCPUCompilerResult res = MCPUAssemblyCompiler.Compile(code.toString());
        
        if (res.Success)
            load(res.Instructions);
        
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
            
            getSign(s -> s.setLine(2, InstructionPointer < instructions.length ? instructions[InstructionPointer].GetOPCode().toString() : "---"));
        }
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
        if (onError != null)
            onError.Raise(this, message);
        
        stop();
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
    
    public String getState()
    {
        return String.format("(%d|%d|%d) created by %s (%s), %s, %s", Location.x, Location.y, Location.z, creator.getDisplayName(), creator.getAddress().toString(), canrun ? "running" :
                                                                                                                                                                            "halted", IsEnabled() ?
                                                                                                                                                                                                  "enabled" :
                                                                                                                                                                                                  "disabled");
    }
    
    public String AssemblyInstructions()
    {
        StringBuilder sb = new StringBuilder();
        
        if (instructions != null)
            for (MCPUInstruction ins : instructions)
                sb.append(ins + "\n");
            
        return sb.toString().trim();
    }
    
    public boolean IsEnabled()
    {
        Block b = world.getBlockAt(x + 1, y + 1, z + 1);
        
        return b.getType() == Material.LEVER ? b.isBlockPowered() || (b.getBlockPower() != 0) : true;
    }

    public int MemorySize()
    {
        return memory == null ? -1 : memory.length;
    }

    @Override
    public int getIOCount()
    {
        return sidecount * 4;
    }

    @Override
    public Location getIOLocation(int port)
    {
        int x = this.x;
        int y = this.y;
        int z = this.z;
        
        if (port < sidecount)
        {
            x += 2 * port;
            z -= 1;
        }
        else if ((port -= sidecount) < sidecount)
        {
            x += sidecount * 2 - 1;
            z += 2 * port;
        }
        else if ((port -= sidecount) < sidecount)
        {
            x += 2 * (sidecount - port - 1);
            z += sidecount * 2 - 1;
        }
        else
        {
            port -= sidecount;
            
            x -= 1;
            z += 2 * (sidecount - port - 1);
        }
        
        return new Location(world, x, y, z);
    }

    
    public World getWorld()
    {
        return world;
    }

    public Location getCenterLocation()
    {
        return new Location(world, x + (xsize - 1) / 2, y, z + (zsize - 1) / 2);
    }
}
