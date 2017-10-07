package com.unknown6656;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.URL;
import java.util.HashMap;
import java.util.function.Consumer;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.bukkit.ChatColor;
import org.bukkit.Material;
import org.bukkit.command.Command;
import org.bukkit.command.CommandSender;
import org.bukkit.craftbukkit.v1_12_R1.inventory.CraftItemStack;
import org.bukkit.entity.Player;
import org.bukkit.plugin.java.JavaPlugin;

import net.minecraft.server.v1_12_R1.ItemStack;
import net.minecraft.server.v1_12_R1.NBTTagCompound;
import net.minecraft.server.v1_12_R1.NBTTagList;


public final class Main extends JavaPlugin
{
    protected final HashMap<Integer, MCPUProcessor> cores = new HashMap<>();
    static String usagetext;
    static Logger log;
    
    
    @Override
    public void onEnable()
    {
        usagetext = "/mcpu command usage:\n" +
                    "  list            - Lists all available MCPU cores\n" +
                    "  add             - Adds a new core to the world\n" +
                    "  remove <n>      - Removes the core No. n\n" +
                    "  loadb <n>       - Loads the book hold in the hand into the core No. n\n" +
                    "  loadu <n> <u>   - Loads the string acessible via the given HTTP URI u into the core No. n\n" +
                    "  start <n>       - Starts the processor core No. n\n" +
                    "  stop <n>        - Halts the core No. n\n" +
                    "  reset <n>       - Halts and resets the core No. n\n" +
                    "  state <n>       - Displays the state of core No. n";
        
        log = getLogger();
        log.log(Level.INFO, MCPUOpcode.Opcodes.size() + " instructions loaded!");
    }
    
    @Override
    public void onDisable()
    {
        for (MCPUProcessor core : cores.values())
            core.Stop();
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
                    sender.sendMessage(ChatColor.YELLOW + usagetext);
                    break;
                case "add":
                {
                    int i = cores.size();
                    
                    cores.put(i, new MCPUProcessor(
                            s -> sender.sendMessage(ChatColor.YELLOW + "Processor " + i + " failed with the folling message:\n" + s)));
                    
                    sender.sendMessage(ChatColor.GREEN + "The core No. " + i + " has been created.");
                }
                    break;
                case "remove":
                    GetInt(args, 1, sender, i ->
                    {
                        if (cores.containsKey(i))
                            cores.remove(i);
                        else
                            sender.sendMessage(ChatColor.RED + "The core No. " + i + " could not be found.");
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
                case "loadb":
                    GetCore(args, 1, sender, c ->
                    {
                        if (sender instanceof Player)
                        {
                            Player player = (Player)sender;
                            org.bukkit.inventory.ItemStack stack = player.getInventory().getItemInMainHand();
                            String[] book = GetBook(stack);
                            
                            if (book == null)
                                book = GetBook(player.getInventory().getItemInOffHand());
                            
                            CompileLoad(c, String.join("\n", book), sender);
                        }
                        else
                            sender.sendMessage(ChatColor.RED + "You must be a player to run this command.");
                    });
                    break;
                case "loadu":
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
                                sender.sendMessage(ChatColor.RED + "The instructions could not be fetched from '" + url + "'.");
                            }
                    });
                    break;
                case "state":
                    GetCore(args, 1, sender, c ->
                    {
                        sender.sendMessage("core: " + c /* TODO */);
                    });
                    break;
                case "list":
                    sender.sendMessage("core  running  name");
                    
                    for (Integer i : cores.keySet())
                    {
                        MCPUProcessor c = cores.get(i);
                        
                        sender.sendMessage("[" + i + "]:  " + c.CanExecuteNext() + "  " + c /* TODO */);
                    }
                    break;
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
                core.Load(res.Instructions);
            else
                sender.sendMessage(ChatColor.RED + "The code could not be loaded into the core due to the following compiler error(s):\n" + res.ErrorMessage);
        }
        catch (Exception e)
        {
            sender.sendMessage(ChatColor.RED + "The code could not be loaded into the core due to the following compiler error(s):\n" + e.getMessage());
        }
    }
    
    private static String[] GetBook(org.bukkit.inventory.ItemStack stack)
    {
        if (stack != null)
            if (stack.getAmount() > 0)
                if ((stack.getType() == Material.BOOK_AND_QUILL) || (stack.getType() == Material.WRITTEN_BOOK))
                {
                    ItemStack st = CraftItemStack.asNMSCopy(stack);
                    
                    if (st.hasTag())
                    {
                        NBTTagCompound tag = st.getTag();
                        NBTTagList pages = (NBTTagList)tag.get("pages");
                        String[] lines = new String[pages.size()];
                        
                        for (int i = 0; i < lines.length; ++i)
                            lines[i] = pages.getString(i);
                        
                        return lines;
                    }
                }
            
        return null;
    }
    
    private void GetCore(final String[] argv, final int ndx, final CommandSender sender, Consumer<MCPUProcessor> action)
    {
        GetInt(argv, ndx, sender, i ->
        {
            if (cores.containsKey(i))
                action.accept(cores.get(i));
            else
                sender.sendMessage(ChatColor.RED + "The core No. " + i + " could not be found.");
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
}
