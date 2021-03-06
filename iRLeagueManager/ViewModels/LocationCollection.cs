﻿// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using iRLeagueManager.Models;

namespace iRLeagueManager.ViewModels
{
    //public class LocationCollection : IList<Location>
    //{
    //    private List<Location> locations;

    //    public LocationCollection()
    //    {
    //        //List<RaceTrack> tracks = new List<RaceTrack>()
    //        //{
    //        //    new RaceTrack(1, "Silverstone"),
    //        //    new RaceTrack(2, "Spa Franchorchamps"),
    //        //    new RaceTrack(3, "Suzuka")
    //        //};
    //        //tracks[0].AddConfig(1, "Stowe Circuit");
    //        //tracks[0].AddConfig(2, "West Layout");
    //        //tracks[0].AddConfig(3, "GP");
    //        //tracks[1].AddConfig(1, "Classic Pits");
    //        //tracks[1].AddConfig(2, "GP");
    //        //tracks[2].AddConfig(1, "West");
    //        //tracks[2].AddConfig(2, "GP");

    //        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<RaceTrack>));
    //        StreamReader streamReader = new StreamReader("C:/Users/simon/Documents/VisualStudio/iRLeagueManager_v0_2/iRLeagueManager_v0_2/bin/Debug/Tracks.xml");
    //        List<RaceTrack> tracks = xmlSerializer.Deserialize(streamReader) as List<RaceTrack>;
    //        tracks.ForEach(x => x.Configs.ForEach(y => y.Track = x));
    //        List<TrackConfig> configs = tracks.Select(x => x.Configs).Aggregate((x, y) => x.Concat(y).ToList());

    //        locations = configs.Select(x => new Location(x)).ToList();
    //    }

    //    public int Count => ((ICollection<Location>)locations).Count;

    //    public bool IsReadOnly => ((ICollection<Location>)locations).IsReadOnly;

    //    public Location this[int index] { get => ((IList<Location>)locations)[index]; set => ((IList<Location>)locations)[index] = value; }

    //    public IEnumerable<RaceTrack> GetTrackList()
    //    {
    //        return locations.Select(x => x.GetTrackInfo()).OrderBy(x => x.TrackName).Distinct();
    //    }

    //    public void Add(Location item)
    //    {
    //        ((ICollection<Location>)locations).Add(item);
    //    }

    //    public void Clear()
    //    {
    //        ((ICollection<Location>)locations).Clear();
    //    }

    //    public bool Contains(Location item)
    //    {
    //        return ((ICollection<Location>)locations).Contains(item);
    //    }

    //    public void CopyTo(Location[] array, int arrayIndex)
    //    {
    //        ((ICollection<Location>)locations).CopyTo(array, arrayIndex);
    //    }

    //    public bool Remove(Location item)
    //    {
    //        return ((ICollection<Location>)locations).Remove(item);
    //    }

    //    public IEnumerator<Location> GetEnumerator()
    //    {
    //        return ((ICollection<Location>)locations).GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return ((ICollection<Location>)locations).GetEnumerator();
    //    }

    //    public int IndexOf(Location item)
    //    {
    //        return ((IList<Location>)locations).IndexOf(item);
    //    }

    //    public void Insert(int index, Location item)
    //    {
    //        ((IList<Location>)locations).Insert(index, item);
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        ((IList<Location>)locations).RemoveAt(index);
    //    }
    //}
}
