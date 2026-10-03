namespace CarShop.Models
{
    public class Car
    {
        public int Id { get; set; }
        public required decimal Price { get; set; }
        public required CarBrand CarBrand { get; set; }
        public required string Model { get; set; }
        public required int Year { get; set; }
        public required bool IsAvailable { get; set; }
        public required string Color { get; set; }
    }

    public class CarBrand
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
    }
}
