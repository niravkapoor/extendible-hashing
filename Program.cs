using ExpendibleHashing;
using Murmur;

ExpendibleHash EH = ExpendibleHash.Instance(MurmurHash.Create128(), 1, 3);

EH.Add("nirav", (1, "india"));
EH.Add("abhishek", (1, "australia"));
EH.Add("deepak", (1, "pakistan"));
EH.Add("ayush", (1, "usa"));
EH.Add("ayush", (2, "usa2"));
EH.Add("Leon", (1, "iran"));
EH.Add("Eliran", (1, "iran", 999999999));

(int, string) Result = EH.FindItem<(int, string)>("abhishek");

Console.WriteLine($"Result for key: abhishek is {Result.Item2}");

(int, string, int) Result2 = EH.FindItem<(int, string, int)>("Eliran");

Console.WriteLine($"Result for key: Eliran is {Result2.Item3}");