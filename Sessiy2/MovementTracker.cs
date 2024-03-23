using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicationHospital;

namespace Sessiy2
{
    internal class MovementTracker
    {
        static public async Task TrackMovements()
        {
            var movements = await APIReader.getPersonMovements();

            // Пример обработки данных и отображения на карте
            foreach (var movement in movements)
            {
                if(movement.LastSecurityPointDirection == "in")
                    Console.WriteLine($"{movement.PersonRole} {movement.PersonCode} вошел в зону {movement.LastSecurityPointNumber}");
                else
                    Console.WriteLine($"{movement.PersonRole} {movement.PersonCode} вышел из зоны {movement.LastSecurityPointNumber}");
            }
        }
    }
}
