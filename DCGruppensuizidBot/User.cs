namespace DGruppensuizidBot;

public struct User
{
    private ulong userId;
    private string username;
    private byte deathPenalty;
    private ushort alphabetcounter;
    private uint alphabetMultCounter;//The User has two different counter.
                                     //The first one is the total amount of wrong spelling, the second one is
                                     //the Counter where other Users can Increase the counter of the Fool by reacting
                                     //to the User as much as possible to its message with something (maybe a fish).
                                     //Save the specific wrong Message (or the link to it) and that to a List IMPORTANT!!!
                                     //
                                     //However some factor have to be considered:
                                     //what happens if a User adds a Fish even if the Fool has Corrected itself?
                                     //What if someone reacts to the Fool, but then the Fool corrects itsself, will the User get a penalty
                                     //for not removing its fish?
                                     //I would say yes, if the Message is within the last 10 Messages it adds a Penalty to the other User
                                     
                                     //TODO: Irgendwann kümmer ich mich darum.
}