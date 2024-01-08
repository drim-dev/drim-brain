var sortedDictionary = new SortedDictionary<int, string>
{
    [5] = "five",
    [1] = "one",
    [3] = "three",
    [2] = "two",
    [4] = "four",
    [9] = "nine",
    [8] = "eight"
};

/*
Outputs:

1 => one
2 => two
3 => three
4 => four
5 => five
8 => eight
9 => nine

Items are sorted by key.
*/
foreach (var (key, value) in sortedDictionary)
{
    Console.WriteLine($"{key} => {value}");
}

/*
Outputs:

Key: 1
Key: 2
Key: 3
Key: 4
Key: 5
Key: 8
Key: 9

Keys are sorted.
*/
foreach (var key in sortedDictionary.Keys)
{
    Console.WriteLine($"Key: {key}");
}

/*
Outputs:

Value: one
Value: two
Value: three
Value: four
Value: five
Value: eight
Value: nine

Values are sorted by corresponding keys.
*/
foreach (var value in sortedDictionary.Values)
{
    Console.WriteLine($"Value: {value}");
}
