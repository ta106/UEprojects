﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using router.com.system;

namespace Router.com
{

    public class router_db
    {
        private HashSet<kid> kids = new HashSet<kid>();
        private HashSet<vehicle> vehicles = new HashSet<vehicle>();
        private SQLiteConnection m_dbConnection;
        private string name;
        private SQLiteConnection con;
        public router_db()
        {
            this.name = "router.sqlite";
           loader(name);

        }

        public router_db(string name)
        {
            this.name = name;
            SQLiteConnection.CreateFile(name);

            con = new SQLiteConnection("data source=" + name + ";");
            con.Open();
            makeTables(con);
            con.Close();
        }

        private void makeTables(SQLiteConnection con)
        {
            string sql = "create table vehicles ( name varchar(20) , capacity int ,primary key(name,capacity));";
            SQLiteCommand cmd = new SQLiteCommand(sql, con);
            cmd.ExecuteNonQuery();
            sql = "create table kids (name varchar(20), address varchar(50), primary key(name, address));";
            cmd = new SQLiteCommand(sql, con);
            cmd.ExecuteNonQuery();
        }

        private void loader(string name)
        {
            m_dbConnection = new SQLiteConnection("data source=" + name + ";"); ;
            m_dbConnection.Open();
            SQLiteDataReader reader;
            string sql = "select * from vehicles;";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            try { reader = command.ExecuteReader();
                while (reader.Read())
                {

                    vehicles.Add(new vehicle(Convert.ToInt32(reader["capacity"]), reader["name"].ToString(), null));
                }
            }catch (SQLiteException e)
            {
                makeTables(m_dbConnection);
            }
            sql = "select * from kids;";
            command =new SQLiteCommand( sql,m_dbConnection);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                kids.Add(new kid(reader["name"].ToString(), reader["address"].ToString()));
            }
            m_dbConnection.Close();
        }

        public void upload(HashSet<kid> kids, HashSet<vehicle> vehicles)
        {
            m_dbConnection = new SQLiteConnection("data source=" + name + ";"); ;
            m_dbConnection.Open();
            // insert every single kid into the database
            foreach (kid each in kids)
            {
                string insertcmd = "Insert into kids values(" + "'" + each.getName() + "'" + "," + "'" + each.getAddress() + "'" + ");";
                try {
                    SQLiteCommand command = new SQLiteCommand(insertcmd, m_dbConnection);
                    command.ExecuteNonQuery();
                }catch(SQLiteException e)
                {
                    // do nothing 
                }
            }


            // insert every single vehicle into the database
            foreach (vehicle each in vehicles)
            {
                string insertcmd = "Insert into vehicles values(" + "'"+each.getName()+"'" + "," +"'"+ each.getCapacity()+"'" + ");";
                try {
                    SQLiteCommand command = new SQLiteCommand(insertcmd, m_dbConnection);
                    command.ExecuteNonQuery();
                }catch (SQLiteException e)
                {
                    // do nothing
                }
            }
            m_dbConnection.Close();
        }

        public void reload()
        {
            kids.Clear();
            vehicles.Clear();
            loader(this.name);
     
        }

        public HashSet<kid> getKids()
        {
            return kids;
        }

        public HashSet<vehicle> getVehicle()
        {
            return vehicles;
        }

    }
}
