//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Test
{
    public class Words
    {
        private static readonly Random s_random = new Random();

        private readonly string[] m_adjectives = { "Abandoned", "Afraid", "Aggressive", "Assorted", "Bawdy", "Befitting", "Blue", "Boring", "Calculating", "Careful", "Cheap", "Clumsy", "Colossal", "Comfortable", "Cruel", "Curious", "Dazzling", "Deafening", "Delicate", "Descriptive", "Different", "Dirty", "Disastrous", "Disgusting", "Disillusioned", "Dry", "Efficacious", "Elastic", "Excellent", "Exclusive", "Fair", "Fertile", "Fine", "Five", "Flimsy", "Fragile", "Frightened", "Glistening", "Good", "Grotesque", "Gusty", "Hideous", "Highfalutin", "Hypnotic", "Kindly", "Labored", "Light", "Long-Term", "Lopsided", "Loving", "Materialistic", "Mighty", "Necessary", "Nonchalant", "Odd", "Overt", "Past", "Penitent", "Picayune", "Poor", "Profuse", "Proud", "Public", "Pushy", "Quirky", "Rampant", "Ripe", "Robust", "Royal", "Rustic", "Secret", "Selfish", "Sharp", "Silly", "Simplistic", "Sincere", "Slimy", "Slippery", "Square", "Squealing", "Sticky", "Strange", "Sturdy", "Teeny", "Testy", "Tightfisted", "True", "Truthful", "Typical", "Unadvised", "Uninterested", "Uttermost", "Various", "Vigorous", "Well-Made", "Windy", "Wiry", "Witty", "Youthful", "Zonked" };
        private readonly string[] m_nouns = { "Achiever", "Act", "Addition", "Apparatus", "Appliance", "Attraction", "Babies", "Basket", "Bird", "Bomb", "Books", "Boot", "Building", "Cabbage", "Cattle", "Celery", "Cherry", "Chess", "Chicken", "Chickens", "Children", "Chin", "Clam", "Clover", "Coach", "Company", "Debt", "Division", "Drain", "Earthquake", "Edge", "Education", "Effect", "Eggs", "Elbow", "Error", "Event", "Fear", "Finger", "Flame", "Flavor", "Flowers", "Grain", "Group", "Hand", "Hobbies", "Kick", "Knee", "Lace", "Lake", "Limit", "Loaf", "Locket", "Lunch", "Mass", "Mice", "Moon", "Morning", "Motion", "Mouth", "Parcel", "Pest", "Planes", "Plate", "Pocket", "Quartz", "Queen", "Quicksand", "Quill", "Reading", "Representative", "Rice", "Roof", "Sail", "Ship", "Skate", "Smoke", "Sort", "Space", "Square", "Stew", "Stick", "Story", "Stove", "Support", "Tendency", "Territory", "Things", "Thread", "Tiger", "Top", "Umbrella", "Voice", "Volleyball", "Voyage", "War", "Window", "Wool", "Word", "Worm" };
        private readonly string[] m_firstNames = { "Aaliyah", "Aaron", "Abigail", "Addison", "Aiden", "Alexander", "Allison", "Amelia", "Andrew", "Anna", "Anthony", "Aria", "Ariana", "Arianna", "Aubrey", "Audrey", "Ava", "Avery", "Benjamin", "Brooklyn", "Caleb", "Camila", "Carter", "Charlotte", "Chloe", "Christian", "Christopher", "Claire", "Daniel", "David", "Dylan", "Eli", "Elijah", "Elizabeth", "Ella", "Emily", "Emma", "Ethan", "Evelyn", "Gabriel", "Gabriella", "Grace", "Hannah", "Harper", "Henry", "Hunter", "Isaac", "Isabella", "Isaiah", "Jack", "Jackson", "Jacob", "James", "Jaxon", "Jayden", "John", "Jonathan", "Joseph", "Joshua", "Julian", "Landon", "Layla", "Leah", "Levi", "Liam", "Lillian", "Lily", "Logan", "Lucas", "Luke", "Madison", "Mason", "Matthew", "Mia", "Michael", "Natalie", "Nathan", "Noah", "Nora", "Oliver", "Olivia", "Owen", "Penelope", "Riley", "Ryan", "Sadie", "Samantha", "Samuel", "Sarah", "Savannah", "Scarlett", "Sebastian", "Skylar", "Sofia", "Sophia", "Victoria", "William", "Wyatt", "Zoe", "Zoey" };
        private readonly string[] m_lastNames = { "Adams", "Allen", "Anderson", "Bailey", "Baker", "Barnes", "Bell", "Bennett", "Brooks", "Brown", "Butler", "Campbell", "Carter", "Clark", "Collins", "Cook", "Cooper", "Cox", "Cruz", "Davis", "Diaz", "Edwards", "Evans", "Fisher", "Flores", "Foster", "Garcia", "Gomez", "Gonzalez", "Gray", "Green", "Gutierrez", "Hall", "Harris", "Hernandez", "Hill", "Howard", "Hughes", "Jackson", "James", "Jenkins", "Johnson", "Jones", "Kelly", "King", "Lee", "Lewis", "Long", "Lopez", "Martin", "Martinez", "Miller", "Mitchell", "Moore", "Morales", "Morgan", "Morris", "Murphy", "Myers", "Nelson", "Nguyen", "Ortiz", "Parker", "Perez", "Perry", "Peterson", "Phillips", "Powell", "Price", "Ramirez", "Reed", "Reyes", "Richardson", "Rivera", "Roberts", "Robinson", "Rodriguez", "Rogers", "Ross", "Russell", "Sanchez", "Sanders", "Scott", "Smith", "Stewart", "Sullivan", "Taylor", "Thomas", "Thompson", "Torres", "Turner", "Walker", "Ward", "Watson", "White", "Williams", "Wilson", "Wood", "Wright", "Young" };

        public string GetRandomAdjective()
        {
            return m_adjectives[s_random.Next(m_adjectives.Length)];
        }

        public string GetRandomNoun()
        {
            return m_nouns[s_random.Next(m_nouns.Length)];
        }

        public string GetRandomFirstName()
        {
            return m_firstNames[s_random.Next(m_firstNames.Length)];
        }

        public string GetRandomLastName()
        {
            return m_lastNames[s_random.Next(m_lastNames.Length)];
        }

        public string GetRandomFullName()
        {
            return GetRandomFirstName() + " " + GetRandomLastName();
        }

        public string GetRandomEmail()
        {
            return GetRandomFirstName() + "." + GetRandomLastName() + "@richtodd.com";
        }

        public string GetRandomStoreName()
        {
            return GetRandomAdjective() + " " + GetRandomNoun();
        }
    }
}
