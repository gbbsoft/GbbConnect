using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GbbConnect.Configuration
{
    public class PlantList : ObservableCollection<Plant>
    {
        //public Plant? Plant { get; internal set; }

        //internal PlantList(Plant plant)
        //{
        //    this.Plant = plant;
        //}

        protected override void InsertItem(int index, Plant item)
        {
            //if (item.Plant != null) throw new ApplicationException("item.Plant!=null");
            //item.Plant = this.Plant;

            // make name unique
            string orgName = item.Name;
            int LP = 2;
            do
            {
                bool OK = true;
                foreach (var itm in this)
                    if (itm.Name == item.Name)
                    {
                        OK = false;
                        break;
                    }
                if (OK) break;

                item.Name = orgName + LP.ToString();
                LP++;

            } while (true);

            base.InsertItem(index, item);
        }

        //protected override void SetItem(int index, Plant item)
        //{
        //    throw new NotImplementedException();
        //    //if (item.Plant != null) throw new ApplicationException("item.Plant!=null");
        //    //this[index].Plant = null;
        //    //item.Plant = this.Plant;
        //}

        protected override void RemoveItem(int index)
        {
            if (Count <= 1)
                throw new ApplicationException("Can't delete last plant!");

            //this[index].Plant = null;
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            throw new NotImplementedException();
            //foreach (var itm in this)
            //    itm.Plant = null;
            //base.ClearItems();
        }
    }
}
