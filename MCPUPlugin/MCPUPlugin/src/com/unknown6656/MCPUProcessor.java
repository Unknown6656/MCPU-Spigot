package com.unknown6656;

import java.util.ArrayList;
import java.util.Stack;
import java.util.function.Consumer;
import java.util.function.Function;

import org.bukkit.entity.Player;

import org.bukkit.Location;
import org.bukkit.Material;
import org.bukkit.World;
import org.bukkit.block.Block;
import org.bukkit.block.BlockState;
import org.bukkit.block.Sign;


public final class MCPUProcessor
{
    private final Stack<MCPUCallframe> callstack = new Stack<>();
    private final ArrayList<Tuple<Byte, Boolean>> ioports; // number, direction (true=out, false=in)
    private final int[] memory = new int[4096];
    private MCPUInstruction[] instructions;
    public final int sideoffset;
    private final int sidecount;
    public int globalscount;
    private boolean canrun;
    private int ticks;
    
    public final Triplet<Integer, Integer, Integer> Location;
    public int InstructionPointer;
    public final Player Creator;
    public final World World;
    
    // C#'s event alternatives -__-
    public MCPUProcessorEvent<MCPUInstruction> OnInstructionExecuted;
    public MCPUProcessorEvent<MCPUInstruction> OnInstructionLoaded;
    public MCPUProcessorEvent<Integer> OnMemoryAccess;
    public MCPUProcessorEvent<Boolean> OnReset;
    public MCPUProcessorEvent<String> OnError;
    public MCPUProcessorEvent<Void> OnStart;
    public MCPUProcessorEvent<Void> OnStop;
    
    
    public MCPUProcessor(Player creator, World world, int x, int y, int z, int count)
    {
        this.Location = new Triplet<Integer, Integer, Integer>(x, y, z);
        this.sideoffset = (count / 2 /* Integer division */) * 2;
        this.ioports = new ArrayList<>(4 * count);
        this.sidecount = count;
        this.Creator = creator;
        this.World = world;
        
        for (int i = 0; i < 4 * count; ++i)
            this.ioports.add(new Tuple<Byte, Boolean>((byte)0, false));
        
        Reset();
    }
    
    public synchronized void Reset()
    {
        Reset(false);
    }
    
    public synchronized void Reset(boolean resetevents)
    {
        Stop();
        
        instructions = new MCPUInstruction[0];
        InstructionPointer = -1;
        callstack.clear();
        
        GetSign(s -> s.setLine(2, ""));
        SetTicks(0);
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
        return canrun && IsEnabled() && (instructions.length > 0) && (InstructionPointer < instructions.length);
    }
    
    public synchronized void ExecuteNext()
    {
        if (CanExecuteNext())
        {
            MCPUInstruction current = instructions[InstructionPointer];
            
            if (current == null)
                ExecuteNext();
            else
            {
                Raise(OnInstructionLoaded, current);
                
                MCPUCallframe frame = callstack.peek();
                MCPUOpcode opcode = current.GetOPCode();
                
                GetSign(s -> s.setLine(2, opcode.toString()));
                
                try
                {
                    if ((opcode.MinimumStackSize() >= 0) && (opcode.MinimumArgumentCount() > frame.StackSize()))
                        Failwith("The execution of the opcode '" + opcode + "' requires at least " + opcode.MinimumArgumentCount() + " values to be on the current stack.");
                    else
                        current.Execute(this, frame);
                    
                    Raise(OnInstructionExecuted, current);
                    
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
                        
                        GetSign(s -> s.setLine(2, opc.toString()));
                    }
                }
            }
            
            SetTicks(ticks + 1);
        }
    }
    
    private void SetTicks(int i)
    {
        ticks = i;
        
        GetSign(s -> s.setLine(3, "T: " + ticks));
    }
    
    private void GetSign(Consumer<Sign> f)
    {
        Block b = World.getBlockAt(Location.x - sideoffset, Location.y + 1, Location.z - sideoffset);
        
        if (b != null)
        {
            Sign s = (Sign)b.getState();
            
            f.accept(s);
            s.update();
        }
    }
    
    public int GetTicks()
    {
        return ticks;
    }
    
    public void IODirection(int port, boolean direction)
    {
        CheckIOExists(port, p -> p.y = direction);
    }
    
    public boolean IODirection(int port)
    {
        return CheckIOExists(port, p -> p.y);
    }
    
    public void IO(int port, int value)
    {
        CheckIOExists(port, p ->
        {
            if (p.y)
            {
                p.x = (byte)Math.max(0, Math.min(value, 15));
                
                // Block b = GetIOPortAdjacent(port).getBlock();
                //
                // if (b.getType() == Material.REDSTONE_WIRE)
                // CraftMagicNumbers.getBlock(b).getBlockData().get(BlockStateInteger.of("power", 0, p.x));
                Location l1 = GetIOPortAdjacent(port);
                Location l2 = l1.clone().add(0, -1, 0);
                
                if (port < sidecount)
                    l1.add(0, 0, 1);
                else if (port < 2 * sidecount)
                    l1.add(-1, 0, 0);
                else if (port < 3 * sidecount)
                    l1.add(0, 0, -1);
                else
                    l1.add(1, 0, 0);
                
                l1.getBlock().setType(value != 0 ? Material.REDSTONE_BLOCK : Material.IRON_BLOCK);
                // l2.getBlock().setType(value != 0 ? Material.REDSTONE_BLOCK : Material.IRON_BLOCK);
            }
            else
                Failwith("The port no. " + port + " is not set for output.");
            
            return null;
        });
    }
    
    public int IO(int port)
    {
        return (int)CheckIOExists(port, p ->
        {
            if (!p.y)
            {
                Block b = GetIOPortAdjacent(port).getBlock();
                BlockState s = b.getState();
                
                s.update();
                
                p.x = (byte)Math.max(0, Math.min(b.getBlockPower(), 15));
                
                return p.x;   
            }
            else
            {
                Failwith("The port no. " + port + " is not set for input.");
                
                return 0;
            }
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
    
    private Location GetIOPortAdjacent(int port)
    {
        int x = Location.x - sideoffset;
        int y = Location.y;
        int z = Location.z - sideoffset;
        
        if (port < sidecount)
        {
            x += 2 * port;
            z -= 2;
        }
        else if ((port -= sidecount) < sidecount)
        {
            x += sidecount * 2;
            z += 2 * port;
        }
        else if ((port -= sidecount) < sidecount)
        {
            x += 2 * (sidecount - port - 1);
            z += sidecount * 2;
        }
        else
        {
            port -= sidecount;
            
            x -= 2;
            z += 2 * (sidecount - port - 1);
        }
        
        return new Location(World, x, y, z);
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
    
    public void Restart()
    {
        Load(instructions);
    }
    
    public void Load(MCPUInstruction[] instr)
    {
        Reset();
        
        if (instructions != null)
        {
            instructions = instr;
            InstructionPointer = 0;
            
            PushCall(new MCPUCallframe());
            
            GetSign(s -> s.setLine(2, InstructionPointer < instructions.length ? instructions[InstructionPointer].GetOPCode().toString() : "---"));
        }
    }
    
    private boolean CheckMemoryAddress(int addr)
    {
        boolean res;
        
        if (res = ((addr < 0) || (addr > memory.length - globalscount)))
            Failwith("Invalid memory access: The address was outside the memory range. The memory address must be a value between 0 and " + (memory.length - globalscount - 1) + ".");
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
        if (OnError != null)
            OnError.Raise(this, message);
        
        Stop();
    }
    
    public void PopCall()
    {
        callstack.pop();
    }
    
    public void PushCall(MCPUCallframe frame)
    {
        callstack.push(frame);
    }
    
    private <T> void Raise(MCPUProcessorEvent<T> evt, T data)
    {
        if (evt != null)
            evt.Raise(this, data);
    }
    
    public String StatusString()
    {
        return String.format("(%d|%d|%d) created by %s (%s), %s, %s", Location.x, Location.y, Location.z, Creator.getDisplayName(), Creator.getAddress().toString(), canrun ? "running" :
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
        Block b = World.getBlockAt(Location.x - sideoffset + 1, Location.y + 1, Location.z - sideoffset);
        
        return b.getType() == Material.LEVER ? b.isBlockPowered() || (b.getBlockPower() != 0) : true;
    }

    public int MemorySize()
    {
        return memory == null ? -1 : memory.length;
    }
}
