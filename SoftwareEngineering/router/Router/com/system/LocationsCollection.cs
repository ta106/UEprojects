using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using router.com.system;

//Author: Zeyu Zhang
//Pair with: Moahmmed Alharthi 
namespace Router.com.system
{   //This class gets addresses info from kid class and makes a 
    //locations collection. Then it can connect to google maps
    //api to get distance between each address.
    public class LocationsCollection
    {
        //This class is a location which contains its address info and
        //distance to other locations +
        //longitude & latitude info
        public class location
        {
            private kid location_info;
            private Dictionary<location, double> distance;
            private double distance_from_dream_center;
            private double longitude;
            private double latitude;
            private int cluster;
            //public bool merge = false;
            //public int num_clusters = -1;
            //public bool hebing = false;

            public location(kid kid_info)
            {
                location_info = new kid(kid_info.getName(), kid_info.getAddress());
                distance = new Dictionary<location, double>();
                cluster = -1;
            }

            public kid get_location_info()
            {
                return location_info;
            }

            public void assign_geocode(double lg, double la)
            {
                longitude = lg;
                latitude = la;
            }

            public double get_longitude()
            {
                return longitude;
            }

            public double get_latitude()
            {
                return latitude;
            }

            public void assign_cluster(int n)
            {
                cluster = n;
            }

            public int get_cluster()
            {
                return cluster;
            }

            public void assign_distance_to_dream_center(double miles)
            {
                distance_from_dream_center = miles;
            }

            public double get_distance_from_dream_center()
            {
                return distance_from_dream_center;
            }

            public void assign_distance_to(location dest, double miles)
            {
                if (!distance.ContainsKey(dest))
                    distance.Add(dest, miles);
                else
                    distance[dest] = miles;
            }

            public void remove_distance_to(location dest)
            {
                if (distance.ContainsKey(dest))
                    distance.Remove(dest);
            }

            public double distance_to(location address)
            {
                if (distance.ContainsKey(address))
                    return distance[address];
                else
                    return -1;
            }

            public void distance_sort()
            {
                distance = distance.OrderBy(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
            }

            public location closest_place()
            {
                return distance.First().Key;
            }

            public override string ToString()
            {
                return this.location_info.ToString();
            }

            public override bool Equals(object obj)
            {
                location target = obj as location;
                if (target == null)
                    return false;
                return (this.location_info.Equals(target.location_info));
            }

            public override int GetHashCode()
            {
                return this.location_info.GetHashCode();
            }
        }


        private List<location> location_list;
        private bool has_got_distance_info;
        private bool has_got_geocode_info;
        public LocationsCollection(List<kid> kids_list)
        {
            location_list = new List<location>();
            has_got_distance_info = false;
            has_got_geocode_info = false;
            foreach (kid each in kids_list)
            {
                location_list.Add(new location(each));
            }
        }

        public List<location> get_location_list()
        {
            return location_list;
        }

        public location find_location(kid location)
        {
            foreach (location each in location_list)
            {
                if (each.get_location_info().Equals(location))
                    return each;
            }
            return null;
        }

        public double parse_distance_json(string json)
        {
            if (json.IndexOf("status") == -1 || json.Substring(json.IndexOf("status") + 11, 2) != "OK")
                return -1;
            string a = json.Substring(json.IndexOf("text") + 9);
            string b = a.Substring(0, a.IndexOf("\""));
            string[] c = b.Split();
            double distance = double.Parse(c[0]);
            if (c[1] == "ft")
                distance *= 0.000189394;
            return distance;
        }

        public void parse_geocode_json(string json, ref double lg, ref double la)
        {
            if (json.IndexOf("status") == -1 || json.Substring(json.IndexOf("status") + 11, 2) != "OK")
            {
                lg = 0;
                la = 0;
                return;
            }
            string a = json.Substring(json.IndexOf("lat") + 7);
            string lat = a.Substring(0, a.IndexOf(","));
            string b = json.Substring(json.IndexOf("lng") + 7);
            string lng = b.Substring(0, b.IndexOf("\n"));
            la = Math.Round(double.Parse(lat), 7);
            lg = Math.Round(double.Parse(lng), 7);
        }

        public bool get_geocode_data_from_api()
        {
            foreach (location A in location_list)
            {
                string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + A.get_location_info().getAddress() +
                 "&key=AIzaSyAJnzEiAdKJvYNQ6Wt6TpavDNLbUMShx5g";
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.ProtocolVersion = new Version(1, 1);
                HttpWebResponse response;
                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                }
                catch
                {
                    return false;
                }

                StreamReader sr = new StreamReader(response.GetResponseStream());
                string result = sr.ReadToEnd();

                double longitude = 0, latitude = 0;
                parse_geocode_json(result, ref longitude, ref latitude);
                if (longitude == 0 && latitude == 0)
                    return false;
                A.assign_geocode(longitude, latitude);
            }
            has_got_geocode_info = true;
            return true;
        }

        public double get_latitude_of(kid address)
        {
            if (has_got_geocode_info == false)
                return -1;
            return find_location(address).get_latitude();
        }

        public double get_longitude_of(kid address)
        {
            if (has_got_geocode_info == false)
                return -1;
            return find_location(address).get_longitude();
        }

        public bool get_distance_based_on_geocode()
        {
            if (!has_got_geocode_info)
            {
                return false;
            }

            string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + "16 W Morgan Ave, Evansville, IN" +
                                "&key=AIzaSyAJnzEiAdKJvYNQ6Wt6TpavDNLbUMShx5g";
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ProtocolVersion = new Version(1, 1);
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch
            {
                return false;
            }

            StreamReader sr = new StreamReader(response.GetResponseStream());
            string result = sr.ReadToEnd();

            double longitude_of_dream_center = 0, latitude_of_dream_center = 0;
            parse_geocode_json(result, ref longitude_of_dream_center, ref latitude_of_dream_center);
            if (longitude_of_dream_center == 0 && latitude_of_dream_center == 0)
                return false;


            for (int i = 0; i < location_list.Count; i++)
            {
                double d_to_dream_center = Math.Sqrt(Math.Pow(location_list[i].get_longitude() - longitude_of_dream_center, 2)
                        + Math.Pow(location_list[i].get_latitude() - latitude_of_dream_center, 2));
                location_list[i].assign_distance_to_dream_center(d_to_dream_center);

                for (int j = i + 1; j < location_list.Count; j++)
                {
                    double d = Math.Sqrt(Math.Pow(location_list[i].get_longitude() - location_list[j].get_longitude(), 2)
                        + Math.Pow(location_list[i].get_latitude() - location_list[j].get_latitude(), 2));
                    location_list[i].assign_distance_to(location_list[j], d);
                    location_list[j].assign_distance_to(location_list[i], d);
                }
                location_list[i].distance_sort();
            }
            return true;
        }

        public bool get_distance_data_from_api()
        {
            for (int i = 0; i < location_list.Count; i++)
            {
                string url = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + location_list[i].get_location_info().getAddress() +
                        "&destinations=16 W Morgan Ave, Evansville, IN" + "&key=AIzaSyAJnzEiAdKJvYNQ6Wt6TpavDNLbUMShx5g";
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.ProtocolVersion = new Version(1, 1);
                HttpWebResponse response;
                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                }
                catch
                {
                    return false;
                }

                StreamReader sr = new StreamReader(response.GetResponseStream());
                string result = sr.ReadToEnd();

                double distance = parse_distance_json(result);
                if (distance < 0)
                    return false;

                location_list[i].assign_distance_to_dream_center(distance);


                for (int j = i + 1; j < location_list.Count; j++)
                {
                    url = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + location_list[i].get_location_info().getAddress() +
                        "&destinations=" + location_list[j].get_location_info().getAddress() + "&key=AIzaSyAJnzEiAdKJvYNQ6Wt6TpavDNLbUMShx5g";
                    request = HttpWebRequest.Create(url) as HttpWebRequest;
                    request.Method = "GET";
                    request.ProtocolVersion = new Version(1, 1);
                    try
                    {
                        response = request.GetResponse() as HttpWebResponse;
                    }
                    catch
                    {
                        return false;
                    }

                    sr = new StreamReader(response.GetResponseStream());
                    result = sr.ReadToEnd();

                    distance = parse_distance_json(result);
                    if (distance < 0)
                        return false;
                    if (!location_list[i].Equals(location_list[j]))
                    {
                        location_list[i].assign_distance_to(location_list[j], distance);
                        location_list[j].assign_distance_to(location_list[i], distance);
                    }
                }
                location_list[i].distance_sort();
            }
            has_got_distance_info = true;
            return true;
        }

        public double distance_between(kid address_A, kid address_B)
        {
            if (has_got_distance_info == false)
                return -1;
            location location_B = new location(address_B);
            return find_location(address_A).distance_to(location_B);
        }
    }
}
