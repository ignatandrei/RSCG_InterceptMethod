using RSCG_InterceptMethodCommon;

namespace RSCG_InterceptMethodConsole;
internal class Person
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    //[InterceptMethod(WhatToIntercept.Timing)]
    public async Task<string> FullName (int sec){

        await Task.Delay(sec);
        return $"{FirstName} {LastName}";
    }

    [InterceptMethod(WhatToIntercept.Timing)]
    public static async Task<string> GetFullStaticName(Person person, int sec)
    {
        return await person.FullName(sec);
    }
}
