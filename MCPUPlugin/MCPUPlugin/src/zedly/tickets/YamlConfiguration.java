/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package zedly.tickets;

import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.Reader;
import java.util.ArrayList;
import java.util.Collection;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import org.yaml.snakeyaml.DumperOptions;
import org.yaml.snakeyaml.Yaml;

/**
 *
 * @author Dennis
 */
public class YamlConfiguration implements Map<String, Object> {

    private final Map<String, Object> map;

    public static YamlConfiguration emptyConfiguration() {
        return new YamlConfiguration(new LinkedHashMap<>());
    }

    public static YamlConfiguration read(File file) {
        try {
            FileReader fr = new FileReader(file);
            return read(fr);
        } catch (IOException ex) {
            return emptyConfiguration();
        }
    }

    public static YamlConfiguration read(InputStream is) {
        InputStreamReader isrdr = new InputStreamReader(is);
        return read(isrdr);
    }

    public static YamlConfiguration read(Reader rdr) {
        try {
            Yaml yaml = new Yaml();
            Object o = yaml.load(rdr);
            if (!(o instanceof Map)) {
                return emptyConfiguration();
            }
            Map<String, Object> map = (Map<String, Object>) o;
            return new YamlConfiguration(map);
        } catch (Exception ex) {
            return emptyConfiguration();
        }
    }

    private YamlConfiguration(Map<String, Object> content) {
        map = content;
    }

    @Override
    public boolean containsKey(Object oPath) {
        return get(oPath) != null;
    }

    public boolean getBoolean(String path, boolean defaultValue) {
        Object value = get(path);
        if (value instanceof Boolean) {
            return (boolean) value;
        } else {
            return defaultValue;
        }
    }

    public double getDouble(String path, double defaultValue) {
        Object value = get(path);
        if (value instanceof Double) {
            return (double) value;
        } else {
            return defaultValue;
        }
    }

    public int getInt(String path, int defaultValue) {
        Object value = get(path);
        if (value instanceof Integer) {
            return (int) value;
        } else {
            return defaultValue;
        }
    }

    public <T> List<T> getList(String path, Class<T> tClass) {
        Object value = get(path);
        if (value instanceof List) {
            try {
                List<T> list = (List<T>) value;
                return list;
            } catch (ClassCastException ex) {
                return null;
            }
        } else {
            return null;
        }
    }

    public long getLong(String path, long defaultValue) {
        Object value = get(path);
        if (value instanceof Long) {
            return (long) value;
        } else if (value instanceof Integer) {
            return (long) (int) value;
        } else {
            return defaultValue;
        }
    }

    public List<YamlConfiguration> getSectionList(String path) {
        List<YamlConfiguration> configs = new ArrayList<>();
        Object value = get(path);
        if (value instanceof List) {
            List list = (List) value;
            for (Object o : list) {
                try {
                    if (o instanceof Map) {
                        configs.add(new YamlConfiguration((Map<String, Object>) o));
                    }
                } catch (ClassCastException ex) {
                }
            }
        }
        return configs;
    }

    public YamlConfiguration getOrCreateSection(String path) {
        Object value = get(path);
        if (value instanceof Map) {
            Map<String, Object> innerMap = (Map<String, Object>) value;
            return new YamlConfiguration(innerMap);
        } else {
            Map<String, Object> newMap = new LinkedHashMap<>();
            set(path, newMap);
            return new YamlConfiguration(newMap);
        }
    }

    public String getString(String path, String defaultValue) {
        Object value = get(path);
        if (value instanceof String) {
            return (String) value;
        } else {
            return defaultValue;
        }
    }

    @Override
    public Object get(Object oPath) {
        if (!(oPath instanceof String)) {
            return null;
        }
        String path = (String) oPath;
        String[] sections = path.split("(?<!\\\\)\\.");
        for (int i = 0; i < sections.length; i++) {
            sections[i] = sections[i].replace("\\.", ".");
        }
        Map<String, Object> innerMap = map;
        for (int i = 0; i < sections.length - 1; i++) {
            Object o = innerMap.get(sections[i]);
            if (o == null) {
                return null;
            } else if (o instanceof Map) {
                innerMap = (Map<String, Object>) o;
            } else {
                return null;
            }
        }
        Object value = innerMap.get(sections[sections.length - 1]);
        if (value instanceof Map) {
            return new YamlConfiguration((Map) value);
        }
        return innerMap.get(sections[sections.length - 1]);
    }

    @Override
    public boolean isEmpty() {
        return map.isEmpty();
    }

    @Override
    public Set<String> keySet() {
        return map.keySet();
    }

    public void save(File file) throws IOException {
        DumperOptions options = new DumperOptions();
        options.setDefaultFlowStyle(DumperOptions.FlowStyle.BLOCK);
        save(file, options);
    }

    public void save(File file, DumperOptions options) throws IOException {
        Yaml yaml = new Yaml(options);
        yaml.dump(map, new FileWriter(file, false));
    }

    public void save(OutputStream stream) throws IOException {
        DumperOptions options = new DumperOptions();
        options.setDefaultFlowStyle(DumperOptions.FlowStyle.BLOCK);
        save(stream, options);
    }

    public void save(OutputStream stream, DumperOptions options) throws IOException {
        Yaml yaml = new Yaml(options);
        yaml.dump(map, new OutputStreamWriter(stream, "UTF-8"));
    }

    @Override
    public Object put(String key, Object value) {
        set(key, value);
        return value;
    }

    public boolean set(String path, Object value) {
        String[] sections = path.split("\\.");
        Map<String, Object> innerMap = map;
        for (int i = 0; i < sections.length - 1; i++) {
            Object o = innerMap.get(sections[i]);
            if (o == null) {
                Map<String, Object> newMap = new LinkedHashMap<>();
                innerMap.put(sections[i], newMap);
                innerMap = newMap;
            } else if (o instanceof Map) {
                innerMap = (Map<String, Object>) o;
            } else {
                System.out.println(o);
                return false;
            }
        }
        if (value instanceof YamlConfiguration) {
            innerMap.put(sections[sections.length - 1], ((YamlConfiguration) value).map);
        } else {
            innerMap.put(sections[sections.length - 1], value);
        }
        return true;
    }

    @Override
    public int size() {
        return map.size();
    }

    @Override
    public String toString() {
        return map.toString();
    }

    @Override
    public boolean containsValue(Object value) {
        return map.containsValue(value);
    }

    @Override
    public Object remove(Object key) {
        return map.remove(key);
    }

    @Override
    public void putAll(Map<? extends String, ? extends Object> m) {
        map.putAll(m);
    }

    @Override
    public void clear() {
        map.clear();
    }

    @Override
    public Collection<Object> values() {
        return map.values();
    }

    @Override
    public Set<Entry<String, Object>> entrySet() {
        return map.entrySet();
    }
}
