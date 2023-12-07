Day5(true);

void Day5(bool part2)
{
    var filePath = @"C:\Users\stefa\source\repos\AOC_5\AOC_5\input.txt";
    var content = File.ReadAllLines(filePath);

    var seeds = new List<long>();
    if (part2)
    {
        seeds = ExtractSeeds_2(content[0]);
    }
    else
    {
        seeds = ExtractSeeds_1(content[0]);
    }

    var processor = new MappingProcessor();
    List<RangeMapping> currentMap = null;


    //Read And Create Mappings
    for (int i = 1; i < content.Length; i++)
    {
        var line = content[i];
        if (line.Contains("map:"))
        {
            if (currentMap != null)
            {
                processor.AddMappingStage(currentMap);
            }
            currentMap = new List<RangeMapping>();
        }
        else if (currentMap != null)
        {
            var parts = line.Split(" ");
            if (parts.Length == 3)
            {
                long destStart = long.Parse(parts[0]);
                long sourceStart = long.Parse(parts[1]);
                long length = long.Parse(parts[2]);
                currentMap.Add(new RangeMapping(destStart, sourceStart, length));
            }

        }
    }

    //Add last mapping after eof
    if (currentMap != null)
        processor.AddMappingStage(currentMap);


    var currSeeds = seeds;
    for (int i = 0; i < processor.MappingStages.Count; i++)
    {
        Console.WriteLine($"Stage {i}:");
        currSeeds = processor.MapStage(currSeeds, i);
    }

    Console.WriteLine($"Final nuzmbers: {string.Join(" ", currSeeds)}");
    Console.WriteLine($"Smallest final number: {currSeeds.Min()}");
}


List<long> ExtractSeeds_1(string seedLine)
{
    var seedParts = seedLine.Substring(6).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    return Array.ConvertAll(seedParts, long.Parse).ToList();
}

List<long> ExtractSeeds_2(string seedline)
{
    var seedParts = seedline.Substring(6).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    var pairs = seedParts
            .Select((value, index) => new { value, index })
            .GroupBy(x => x.index / 2)
            .Select(group => (
                first: long.Parse(group.ElementAt(0).value),
                second: long.Parse(group.ElementAt(1).value)))
            .ToList();



    //Enumerable.Range() sadly only supports ints ;( But seeds are > Int.Max

    var allSeeds = new List<long>();
    foreach (var pair in pairs)
    {
        for (int i = 0; i < pair.second ; i++)
        {
            allSeeds.Add(pair.first + i);
        }
    }

    return allSeeds;
}

public class RangeMapping
{
    public long SourceStart { get; set; }
    public long DestStart { get; set; }
    public long Length { get; set; }

    public RangeMapping(long destStart, long sourceStart, long length)
    {
        SourceStart = sourceStart;
        DestStart = destStart;
        Length = length;
    }

    public long? Map(long source)
    {
        if (source >= SourceStart && source < SourceStart + Length)
        {
            return DestStart + (source - SourceStart);
        }

        return null;
    }
}

public class MappingProcessor
{
    public List<List<RangeMapping>> MappingStages { get; set; } = new List<List<RangeMapping>>();

    public void AddMappingStage(List<RangeMapping> stageMappings)
    {
        MappingStages.Add(stageMappings);
    }
    
    public List<long> MapStage(List<long> seeds, int stage)
    {
        
        var mappedList = new List<long>();
        foreach (var seed in seeds)
        {
            var mapped = seed;

            foreach (var Mapping in MappingStages[stage])
            {
                var result = Mapping.Map(seed);
                if (result != null)
                {
                    mapped = result.Value;
                }
            }

            mappedList.Add(mapped);
        } 
        return mappedList;
    }
}



