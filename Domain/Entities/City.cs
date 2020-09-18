namespace Domain.Entities
{
    using System.Collections.Generic;

    public class City 
    {
        public City()
        {
            Districts = new List<District>();
        }

        public int Id { get; set; }

        public string Name { get; set; }


        public IList<District> Districts { get; set; }
    }
}
