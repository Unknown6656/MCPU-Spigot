
package com.unknown6656;


import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.URL;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.function.Consumer;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.bukkit.ChatColor;
import org.bukkit.DyeColor;
import org.bukkit.Location;
import org.bukkit.Material;
import org.bukkit.World;
import org.bukkit.block.Block;
import org.bukkit.block.BlockFace;
import org.bukkit.block.Sign;
import org.bukkit.command.Command;
import org.bukkit.command.CommandSender;
import org.bukkit.entity.Player;
import org.bukkit.event.EventHandler;
import org.bukkit.event.Listener;
import org.bukkit.event.block.BlockBreakEvent;
import org.bukkit.event.block.BlockPlaceEvent;
import org.bukkit.event.world.WorldInitEvent;
import org.bukkit.event.world.WorldLoadEvent;
import org.bukkit.event.world.WorldSaveEvent;
import org.bukkit.inventory.ItemStack;
import org.bukkit.inventory.meta.BookMeta;
import org.bukkit.material.Lever;
import org.bukkit.plugin.java.JavaPlugin;


// Lamdas: finally java can do SOMETHING that .net could do since at least
// 1933.....

public final class Main extends JavaPlugin implements Listener
{
    public static final LinkedList<Triplet<Integer, Integer, Integer>> pendingexplosions = new LinkedList<>();
    public static final HashMap<Integer, MCPUBuildRegion> regions = new HashMap<>();
    public static final HashMap<Integer, MCPUProcessor> cores = new HashMap<>();
    private static final int CPUSIZE = 3; // TODO fix
    static String usagetext;
    static Logger log;
    
    
    @Override
    public void onEnable()
    {
        usagetext = "/mcpu command usage:\n" +
                    " list              - Lists all available MCPU cores\n" +
                    " add               - Adds a new core to the world at the callers position\n" +
                    " add <x> <y> <z>   - Adds a new core to the world at the given coordinates\n" +
                    " remove <n>        - Removes the core No. n\n" +
                    " loadb[ook] <n>    - Loads the book hold in the hand into the core No. n\n" +
                    " loadu[rl] <n> <u> - Loads the string acessible via the given HTTP URI u into the core No. n\n" +
                    " start <n>         - Starts the processor core No. n\n" +
                    " stop <n>          - Halts the core No. n\n" +
                    " next <n>          - Forces the execution of the next instruction of core No. n" +
                    " reset <n>         - Halts and resets the core No. n\n" +
                    " state <n>         - Displays the state of core No. n";
        
        log = getLogger();
        log.log(Level.INFO, MCPUOpcode.Opcodes.size() + " instructions loaded!");
        
        getServer().getPluginManager().registerEvents(this, this);
        getServer().getScheduler().scheduleSyncRepeatingTask(this, () ->
        {
            synchronized (cores)
            {
                for (MCPUProcessor c : cores.values())
                    c.ExecuteNext();
            }
        }, 1, 1);
    }
    
    @Override
    public void onDisable()
    {
        for (MCPUProcessor core : cores.values())
            core.Stop();
    }
    
    @EventHandler
    public void onBlockPlaceEvent(BlockPlaceEvent event)
    {
        Location l = event.getBlock().getLocation();
        
        for (MCPUBuildRegion reg : regions.values())
            if (reg.Contains(l))
            {
                event.setCancelled(true);
                
                if (event.getPlayer() != null)
                    event.getPlayer().sendMessage(ChatColor.RED + "You cannot place a block on a registered CPU core.");
            }
    }
    
    @EventHandler
    public void onBlockBreakEvent(BlockBreakEvent event)
    {
        Location l = event.getBlock().getLocation();
        
        for (MCPUBuildRegion reg : regions.values())
            if (reg.Contains(l))
            {
                event.setCancelled(true);
                
                if (event.getPlayer() != null)
                    event.getPlayer().sendMessage(ChatColor.RED + "You cannot destroy a block from a registered CPU core.");
            }
    }
    
    public void onWorldSaveEvent(WorldSaveEvent event)
    {
        
    }
    
    public void onWorldLoadEvent(WorldLoadEvent event)
    {
        
    }
    
    public void onWorldInitEvent(WorldInitEvent event)
    {
        
    }
    
    @Override
    public boolean onCommand(CommandSender sender, Command command, String label, String[] args)
    {
        if (command.getName().equalsIgnoreCase("mcpu"))
        {
            if (args.length == 0)
                args = new String[] { "?" };
            
            switch (args[0].toLowerCase().trim())
            {
                case "?":
                case "help":
                    Print(sender, ChatColor.YELLOW, usagetext);
                    break;
                case "add":
                {
                    Player player = null;
                    boolean canbuild = true;
                    int x, y, z;
                    
                    if (sender instanceof Player)
                    {
                        player = (Player)sender;
                        
                        Location loc = player.getLocation();
                        
                        x = loc.getBlockX();
                        y = loc.getBlockY();
                        z = loc.getBlockZ();
                    }
                    else
                        try
                        {
                            x = Integer.parseInt(args[1]);
                            y = Integer.parseInt(args[2]);
                            z = Integer.parseInt(args[3]);
                        }
                        catch (Exception e)
                        {
                            Print(sender, ChatColor.RED, "You must provide valid x-, y- and z-corrdinates for the creation of a new core.");
                            
                            break;
                        }
                    
                    synchronized (cores)
                    {
                        double dist = (CPUSIZE + 2) * 2;
                        
                        for (int i : cores.keySet())
                        {
                            MCPUProcessor core = cores.get(i);
                            Triplet<Integer, Integer, Integer> l = core.Location;
                            
                            if (Math.abs(l.y - y) < 3)
                                if (Math.sqrt(Math.pow(l.x - x, 2) + Math.pow(l.y - y, 2) + Math.pow(l.z - z, 2)) < dist)
                                {
                                    Print(sender, ChatColor.RED, "The new processor core cannot be placed here. It would be to close to existing core no. " + i + ".");
                                    
                                    canbuild = false;
                                    
                                    break;
                                }
                        }
                    }
                    
                    if (canbuild)
                    {
                        Tuple<Integer, MCPUProcessor> t = SpawnCPU(player, x, y, z, CPUSIZE);
                        
                        t.y.OnError = (p, s) -> Print(sender, ChatColor.YELLOW, "Processor " + t.x + " failed with the folling message:\n" + s);
                        
                        Print(sender, ChatColor.GREEN, "The core No. " + t.x + " has been created.");
                    }
                }
                    break;
                case "delete":
                case "remove":
                    GetInt(args, 1, sender, i ->
                    {
                        synchronized (cores)
                        {
                            if (cores.containsKey(i))
                            {
                                MCPUProcessor c = cores.remove(i);
                                
                                DeleteCPU(c.World, c.Location.x, c.Location.y, c.Location.z, CPUSIZE);
                                
                                regions.remove(i);
                            }
                            else
                                Print(sender, ChatColor.RED, "The core No. " + i + " could not be found.");
                        }
                    });
                    break;
                case "reset":
                    GetCore(args, 1, sender, c -> c.Reset());
                    break;
                case "stop":
                    GetCore(args, 1, sender, c -> c.Stop());
                    break;
                case "start":
                    GetCore(args, 1, sender, c -> c.Start());
                    break;
                case "next":
                    GetCore(args, 1, sender, c -> c.ExecuteNext());
                    break;
                case "loadb":
                case "loadbook":
                    GetCore(args, 1, sender, c ->
                    {
                        if (sender instanceof Player)
                        {
                            Player player = (Player)sender;
                            ItemStack stack = player.getInventory().getItemInMainHand();
                            String[] book = GetBook(stack);
                            
                            if (book == null)
                                book = GetBook(player.getInventory().getItemInOffHand());
                            
                            CompileLoad(c, String.join("\n", book), sender);
                        }
                        else
                            Print(sender, ChatColor.RED, "You must be a player to run this command.");
                    });
                    break;
                case "loadu":
                case "loadurl":
                    final String[] argv = args; // IDE throws some obscure error
                                                // if I replace 'argv' with
                                                // 'args' in this block
                    
                    GetCore(args, 1, sender, c ->
                    {
                        String url = GetArg(argv, 2, sender);
                        StringBuilder code = new StringBuilder();
                        
                        if (url != null)
                            try
                            {
                                String s = null;
                                URL uri = new URL(url);
                                BufferedReader reader = new BufferedReader(new InputStreamReader(uri.openStream()));
                                
                                while ((s = reader.readLine()) != null)
                                    code.append('\n').append(s);
                                
                                CompileLoad(c, code.toString(), sender);
                            }
                            catch (Exception e)
                            {
                                Print(sender, ChatColor.RED, "The instructions could not be fetched from '" + url + "'.");
                            }
                    });
                    break;
                case "state":
                    GetCore(args, 1, sender, c -> sender.sendMessage(c.StatusString()));
                    break;
                case "list":
                    for (Integer i : cores.keySet())
                    {
                        MCPUProcessor c = cores.get(i);
                        
                        sender.sendMessage("[" + i + "]: " + c.StatusString());
                    }
                    break;
                default:
                    Print(sender, ChatColor.RED, "The command '" + args[0].trim() + "' is unknown.");
            }
            
            return true;
        }
        else
            return false;
    }
    
    private void CompileLoad(MCPUProcessor core, String code, CommandSender sender)
    {
        try
        {
            MCPUCompilerResult res = MCPUAssemblyCompiler.Compile(code);
            
            if (res.Success)
            {
                core.Load(res.Instructions);
                
                sender.sendMessage(ChatColor.GREEN + "The code has been compiled to " + res.Instructions.length + " instructions and was successfully loaded into the core.");
            }
            else
                sender.sendMessage(ChatColor.RED + "The code could not be loaded into the core due to the following compiler error(s):\n" + res.ErrorMessage);
        }
        catch (Exception e)
        {
            sender.sendMessage(ChatColor.RED + "The code could not be loaded into the core due to the following compiler error(s):\n" + e.getMessage());
        }
    }
    
    private static String[] GetBook(ItemStack stack)
    {
        if (stack != null)
            if (stack.getAmount() > 0)
                if (((stack.getType() == Material.BOOK_AND_QUILL) || (stack.getType() == Material.WRITTEN_BOOK)) && stack.hasItemMeta())
                {
                    BookMeta bm = (BookMeta)stack.getItemMeta();
                    String[] lines = new String[0];
                    
                    return bm.getPages().toArray(lines);
                }
            
        return null;
    }
    
    private void GetCore(final String[] argv, final int ndx, final CommandSender sender, Consumer<MCPUProcessor> action)
    {
        GetInt(argv, ndx, sender, i ->
        {
            synchronized (cores)
            {
                if (cores.containsKey(i))
                    action.accept(cores.get(i));
                else
                    sender.sendMessage(ChatColor.RED + "The core No. " + i + " could not be found.");
            }
        });
    }
    
    private static void GetInt(final String[] argv, final int ndx, final CommandSender sender, Consumer<Integer> action)
    {
        String arg = GetArg(argv, ndx, sender);
        
        if (arg != null)
            try
            {
                action.accept(Integer.parseInt(arg));
            }
            catch (Exception e)
            {
                sender.sendMessage(ChatColor.RED + "The argument " + (ndx + 1) + " could not be interpreted as a numeric value.");
            }
    }
    
    private static String GetArg(final String[] argv, final int ndx, final CommandSender sender)
    {
        if (ndx < argv.length)
            return argv[ndx];
        else
            sender.sendMessage(ChatColor.RED + "At least " + (argv.length - ndx + 1) + " more argument(s) are required for this command.");
        
        return null;
    }
    
    private void DeleteCPU(World w, int x, int y, int z, int size)
    {
        int sdhl = (size / 2 /* Integer division */) * 2;
        
        for (int i = -sdhl - 2; i <= sdhl + 2; ++i)
            for (int j = -sdhl - 2; j <= sdhl + 2; ++j)
                for (int k = 3; k >= -1; --k)
                    SetBlock(w, x + i, y + k, z + j, Material.AIR);
    }
    
    @SuppressWarnings("deprecation")
    private Tuple<Integer, MCPUProcessor> SpawnCPU(Player p, int x, int y, int z, int size)
    {
        World w = p.getWorld();
        
        synchronized (cores)
        {
            int num = cores.size();
            int sdhl = (size / 2 /* Integer division */) * 2;
            
            DeleteCPU(w, x, y, z, size);
            
            for (int i = -sdhl - 2; i <= sdhl + 2; ++i)
                for (int j = -sdhl - 2; j <= sdhl + 2; ++j)
                    SetBlock(w, x + i, y - 1, z + j, Material.STONE);
                
            for (int i = -sdhl; i <= sdhl; ++i)
                for (int j = -sdhl; j <= sdhl; ++j)
                    SetBlock(w, x + i, y, z + j, Material.WOOL, b -> b.setData(DyeColor.BLACK.getWoolData())); // TODO: fix deprecated calls
                    
            SetBlock(w, x - sdhl, y, z - sdhl, Material.GOLD_BLOCK);
            SetBlock(w, x - sdhl + 1, y, z - sdhl, Material.GOLD_BLOCK);
            
            for (int i = -sdhl; i <= sdhl; i += 2)
            {
                SetBlock(w, x + i, y, z - sdhl - 1, Material.IRON_BLOCK);
                SetBlock(w, x + i, y, z + sdhl + 1, Material.IRON_BLOCK);
                // SetBlock(w, x + i, y - 1, z - sdhl - 2, Material.IRON_BLOCK);
                // SetBlock(w, x + i, y - 1, z + sdhl + 2, Material.IRON_BLOCK);
                SetBlock(w, x + i, y, z - sdhl - 2, Material.REDSTONE_WIRE);
                SetBlock(w, x + i, y, z + sdhl + 2, Material.REDSTONE_WIRE);
            }
            
            for (int j = -sdhl; j <= sdhl; j += 2)
            {
                SetBlock(w, x - sdhl - 1, y, z + j, Material.IRON_BLOCK);
                SetBlock(w, x + sdhl + 1, y, z + j, Material.IRON_BLOCK);
                // SetBlock(w, x - sdhl - 2, y - 1, z + j, Material.IRON_BLOCK);
                // SetBlock(w, x + sdhl + 2, y - 1, z + j, Material.IRON_BLOCK);
                SetBlock(w, x - sdhl - 2, y, z + j, Material.REDSTONE_WIRE);
                SetBlock(w, x + sdhl + 2, y, z + j, Material.REDSTONE_WIRE);
            }
            
            SetBlock(w, x - sdhl + 1, y + 1, z - sdhl, Material.AIR, b ->
            {
                b.setType(Material.LEVER);
                
                Lever lever = (Lever)b.getState().getData();
                
                lever.setFacingDirection(BlockFace.UP);
                lever.setPowered(true);
                
                b.getState().setData(lever);
                b.setData((byte)6);
                
                b.getState().update();
            });
            SetBlock(w, x - sdhl, y + 1, z - sdhl, Material.SIGN_POST, b ->
            {
                Sign sign = (Sign)b.getState();
                
                sign.setLine(0, "CPU No. " + num);
                sign.setLine(1, p.getDisplayName());
                sign.update();
            });
            
            // BUILD STUFF
            
            MCPUProcessor proc = new MCPUProcessor(p, w, x, y, z, size);
            
            cores.put(num, proc);
            regions.put(num, new MCPUBuildRegion(x, y, z, sdhl + 1));
            
            return new Tuple<Integer, MCPUProcessor>(num, proc);
        }
    }
    
    private static Block SetBlock(World w, int x, int y, int z, Material m)
    {
        Block b = new Location(w, x, y, z).getBlock();
        
        b.setType(m);
        
        return b;
    }
    
    private static void SetBlock(World w, int x, int y, int z, Material m, Consumer<Block> f)
    {
        f.accept(SetBlock(w, x, y, z, m));
    }
    
    public static void Print(ChatColor c, String m)
    {
        Print(null, c, m);
    }
    
    public static void Print(CommandSender s, ChatColor c, String m)
    {
        if (s != null)
            s.sendMessage(c + m);
        
        log.log(Level.INFO, c + m);
    }
}
