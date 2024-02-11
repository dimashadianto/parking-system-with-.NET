using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            ParkingLot parkingLot = null;
            string command;

            do
            {
                Console.Write("$ ");
                command = Console.ReadLine();
                string[] parts = command.Split(' ');

                switch (parts[0])
                {
                    case "create_parking_lot":
                        int capacity = int.Parse(parts[1]);
                        parkingLot = new ParkingLot(capacity);
                        Console.WriteLine($"Created a parking lot with {capacity} slots");
                        break;

                    case "park":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Parking lot is not created yet.");
                            break;
                        }

                        string registrationNumber = parts[1];
                        string color = parts[2];
                        string type = parts[3];
                        VehicleType vehicleType = type.ToLower() == "mobil" ? VehicleType.Car : VehicleType.Motorcycle;
                        if (parkingLot.IsFull())
                        {
                            Console.WriteLine("Sorry, parking lot is full");
                            break;
                        }

                        int slotNumber = parkingLot.CheckIn(new Vehicle(registrationNumber, color, vehicleType));
                        Console.WriteLine($"Allocated slot number: {slotNumber}");
                        break;

                    case "leave":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Parking lot is not created yet.");
                            break;
                        }

                        int leavingSlot = int.Parse(parts[1]);
                        parkingLot.CheckOut(leavingSlot);
                        Console.WriteLine($"Slot number {leavingSlot} is free");
                        break;

                    case "status":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Parking lot is not created yet.");
                            break;
                        }

                        Console.WriteLine("Slot No.   Registration No    Colour     Type");
                        foreach (var slot in parkingLot.GetOccupiedSlots())
                        {
                            Vehicle vehicle = parkingLot.GetVehicleInSlot(slot);
                            Console.WriteLine($"{slot,-10}{vehicle.RegistrationNumber,-18}{vehicle.Color,-10}{vehicle.Type,-10}");
                        }
                        break;

                    case "type_of_vehicles":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Parking lot is not created yet.");
                            break;
                        }

                        string vehicleTypeString = parts[1].ToLower();
                        VehicleType typeToCheck = vehicleTypeString == "mobil" ? VehicleType.Car : VehicleType.Motorcycle;
                        int count = parkingLot.GetVehicleCountByType(typeToCheck);
                        Console.WriteLine(count);
                        break;

                    case "registration_numbers_for_vehicles_with_odd_plate":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Parking lot is not created yet.");
                            break;
                        }

                        var oddPlateNumbers = parkingLot.GetRegistrationNumbersWithOddPlate();
                        Console.WriteLine(string.Join(", ", oddPlateNumbers));
                        break;

                    case "registration_numbers_for_vehicles_with_even_plate":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Parking lot is not created yet.");
                            break;
                        }

                        var evenPlateNumbers = parkingLot.GetRegistrationNumbersWithEvenPlate();
                        Console.WriteLine(string.Join(", ", evenPlateNumbers));
                        break;

                    case "registration_numbers_for_vehicles_with_color":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Parking lot is not created yet.");
                            break;
                        }

                        string colorToCheck = parts[1];
                        var registrationNumbersByColor = parkingLot.GetRegistrationNumbersByColor(colorToCheck);
                        Console.WriteLine(string.Join(", ", registrationNumbersByColor));
                        break;

                    case "slot_numbers_for_vehicles_with_color":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Parking lot is not created yet.");
                            break;
                        }

                        string colorForSlots = parts[1];
                        var slotNumbersByColor = parkingLot.GetSlotNumbersByColor(colorForSlots);
                        Console.WriteLine(string.Join(", ", slotNumbersByColor));
                        break;

                    case "slot_number_for_registration_number":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Parking lot is not created yet.");
                            break;
                        }

                        string registrationNumberToSearch = parts[1];
                        int slotNumberForRegistration = parkingLot.GetSlotNumberByRegistrationNumber(registrationNumberToSearch);
                        if (slotNumberForRegistration != 0)
                            Console.WriteLine(slotNumberForRegistration);
                        else
                            Console.WriteLine("Not found");
                        break;

                    case "exit":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }

            } while (true);
        }
    }

    public enum VehicleType
    {
        Car,
        Motorcycle
    }

    public class Vehicle
    {
        public string RegistrationNumber { get; }
        public string Color { get; }
        public VehicleType Type { get; }

        public Vehicle(string registrationNumber, string color, VehicleType type)
        {
            RegistrationNumber = registrationNumber;
            Color = color;
            Type = type;
        }
    }

    public class ParkingLot
    {
        private int Capacity { get; }
        private Dictionary<int, Vehicle> OccupiedSlots { get; }

        public ParkingLot(int capacity)
        {
            Capacity = capacity;
            OccupiedSlots = new Dictionary<int, Vehicle>();
        }

        public bool IsFull()
        {
            return OccupiedSlots.Count >= Capacity;
        }

        public int CheckIn(Vehicle vehicle)
        {
            int slot = FindNextAvailableSlot();
            OccupiedSlots[slot] = vehicle;
            return slot;
        }

        public void CheckOut(int slotNumber)
        {
            OccupiedSlots.Remove(slotNumber);
        }

        public List<int> GetOccupiedSlots()
        {
            return OccupiedSlots.Keys.ToList();
        }

        public Vehicle GetVehicleInSlot(int slotNumber)
        {
            return OccupiedSlots[slotNumber];
        }

        public int GetVehicleCountByType(VehicleType type)
        {
            return OccupiedSlots.Count(v => v.Value.Type == type);
        }

        public List<string> GetRegistrationNumbersWithOddPlate()
        {
            return OccupiedSlots.Where(v => v.Value.RegistrationNumber.Last() % 2 != 0).Select(v => v.Value.RegistrationNumber).ToList();
        }

        public List<string> GetRegistrationNumbersWithEvenPlate()
        {
            return OccupiedSlots.Where(v => v.Value.RegistrationNumber.Last() % 2 == 0).Select(v => v.Value.RegistrationNumber).ToList();
        }

        public List<string> GetRegistrationNumbersByColor(string color)
        {
            return OccupiedSlots.Where(v => v.Value.Color.Equals(color, StringComparison.OrdinalIgnoreCase)).Select(v => v.Value.RegistrationNumber).ToList();
        }

        public List<int> GetSlotNumbersByColor(string color)
        {
            return OccupiedSlots.Where(v => v.Value.Color.Equals(color, StringComparison.OrdinalIgnoreCase)).Select(v => v.Key).ToList();
        }

        public int GetSlotNumberByRegistrationNumber(string registrationNumber)
        {
            var slot = OccupiedSlots.FirstOrDefault(v => v.Value.RegistrationNumber.Equals(registrationNumber, StringComparison.OrdinalIgnoreCase));
            return slot.Key;
        }

        private int FindNextAvailableSlot()
        {
            for (int i = 1; i <= Capacity; i++)
            {
                if (!OccupiedSlots.ContainsKey(i))
                    return i;
            }
            throw new InvalidOperationException("Parking lot is full");
        }
    }
}