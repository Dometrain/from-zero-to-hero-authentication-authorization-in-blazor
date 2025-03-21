using BlazorWebAppBuilInAuthentication.Client.SuperHeroDemo;

namespace BlazorWebAppDemoWithAuth;

public class SuperHeroRepository
{
    private List<Superhero> SuperheroList;
    public List<Superhero> GetData()
    {
        return SuperheroList;
    }

    public int GetDataCount()
    {
        return SuperheroList.Count;
    }

    public Task<List<Superhero>> GetDataAsync(int startIndex,int numberofrecords, CancellationToken cancelationToken)
    {
        return Task.FromResult(SuperheroList.Skip(startIndex).Take(numberofrecords).ToList());
    }

    public SuperHeroRepository()
    {
        SuperheroList = new List<Superhero>()
        {
            new Superhero { Id = 1, Name = "Spider-Man", RealName = "Peter Parker", Powers = "Spider sense, wall-crawling, super strength" },
            new Superhero { Id = 2, Name = "Wonder Woman", RealName = "Diana Prince", Powers = "Super strength, agility, lasso of truth" },
            new Superhero { Id = 3, Name = "Batman", RealName = "Bruce Wayne", Powers = "Expert detective, martial arts skills" },
            new Superhero { Id = 4, Name = "Black Widow", RealName = "Natasha Romanoff", Powers = "Expert spy, martial artist" },
            new Superhero { Id = 5, Name = "The Flash", RealName = "Barry Allen", Powers = "Super speed, time travel" },
            new Superhero { Id = 6, Name = "Green Lantern", RealName = "Hal Jordan", Powers = "Power ring that creates energy constructs" },
            new Superhero { Id = 7, Name = "Thor", RealName = "Thor Odinson", Powers = "Control over thunder, super strength" },
            new Superhero { Id = 8, Name = "Iron Man", RealName = "Tony Stark", Powers = "Genius level intellect, armored suit" },
            new Superhero { Id = 9, Name = "Steve Sanderson", RealName = "Steve Sanderson", Powers = "Creation of Blazor, software innovation" },
            new Superhero { Id = 10, Name = "Hulk", RealName = "Bruce Banner", Powers = "Super strength, regeneration" },
            new Superhero { Id = 11, Name = "Hawkeye", RealName = "Clint Barton", Powers = "Master archer" },
            new Superhero { Id = 12, Name = "Cyclops", RealName = "Scott Summers", Powers = "Optic blast" },
            new Superhero { Id = 13, Name = "Storm", RealName = "Ororo Munroe", Powers = "Weather manipulation" },
            new Superhero { Id = 14, Name = "Black Panther", RealName = "T'Challa", Powers = "Enhanced senses, superhuman condition" },
            new Superhero { Id = 15, Name = "Aquaman", RealName = "Arthur Curry", Powers = "Underwater breathing, communicates with sea creatures" },
            new Superhero { Id = 16, Name = "Phoenix", RealName = "Jean Grey", Powers = "Telepathy, telekinesis" },
            new Superhero { Id = 17, Name = "Silver Surfer", RealName = "Norrin Radd", Powers = "Power cosmic, space travel" },
            new Superhero { Id = 18, Name = "Doctor Strange", RealName = "Stephen Strange", Powers = "Sorcery, dimensional travel" },
            new Superhero { Id = 19, Name = "Daredevil", RealName = "Matt Murdock", Powers = "Superhuman senses, acrobatic ability" },
            new Superhero { Id = 20, Name = "Punisher", RealName = "Frank Castle", Powers = "Expert marksman, skilled fighter" },
            new Superhero { Id = 21, Name = "Deadpool", RealName = "Wade Wilson", Powers = "Regeneration, expert marksman" },
            new Superhero { Id = 22, Name = "Nightcrawler", RealName = "Kurt Wagner", Powers = "Teleportation, acrobatic agility" },
            new Superhero { Id = 23, Name = "Wolverine", RealName = "Logan", Powers = "Regeneration, retractable claws" },
            new Superhero { Id = 24, Name = "Ghost Rider", RealName = "Johnny Blaze", Powers = "Hellfire manipulation, super strength" },
            new Superhero { Id = 25, Name = "Captain Marvel", RealName = "Carol Danvers", Powers = "Super strength, flight, energy projection" },
            new Superhero { Id = 26, Name = "Scarlet Witch", RealName = "Wanda Maximoff", Powers = "Reality manipulation, telekinesis" },
            new Superhero { Id = 27, Name = "Vision", RealName = "Vision", Powers = "Density control, solar energy beams" },
            new Superhero { Id = 28, Name = "Ant-Man", RealName = "Scott Lang", Powers = "Size manipulation, communicates with insects" },
            new Superhero { Id = 29, Name = "Doctor Fate", RealName = "Kent Nelson", Powers = "Magic, superhuman strength" },
            new Superhero { Id = 30, Name = "Zatanna", RealName = "Zatanna Zatara", Powers = "Spellcasting, illusions" },
            new Superhero { Id = 31, Name = "Captain America", RealName = "Steve Rogers", Powers = "Super soldier serum, expert tactician" },
            new Superhero { Id = 32, Name = "Shazam", RealName = "Billy Batson", Powers = "Magical powers, transformation by saying 'Shazam'" },
            new Superhero { Id = 33, Name = "Mystique", RealName = "Raven Darkholme", Powers = "Shapeshifting" }
        };
    }
}
