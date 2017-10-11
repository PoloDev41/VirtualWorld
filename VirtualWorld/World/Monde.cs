using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualWorld;
using VirtualWorld.World;
using Microsoft.Xna.Framework.Content;
using MonoGameConsole;
using System.Threading.Tasks;
using VirtualWorld.World.Actors;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using WinForm_DataView;

namespace VirtualWorld
{
    public enum SeasonTournament
    {
        SAISON1,
        SAISON2,
        SAISON1_2,
        SAISON2_1
    }

    [Serializable]
    public class Monde : IConsoleCommand
    {
        enum ViewType
        {
            ALTITUDE,
            TEMPERATURE,
            ENGRAIS,
            NORMAL
        }

        public float TemperatureMax
        {
            get;
            set;
        }

        public float TemperatureMin
        {
            get;
            set;
        }

        public float AltitudeMax { get; set; }
        public float AltitudeMin { get; set; }

        public static readonly int TimeSeason = 20;
        public static readonly int TimeInterSeason = 60;
        public static readonly int TimeChangementGlobalWarming = (TimeSeason * 2 + TimeInterSeason * 2) * 1;
        public static Random rand = new Random();

        private bool _viewPlante = true;
        private ViewType _view = ViewType.NORMAL;
        private float _timeSaison = TimeSeason;

        /// <summary>
        /// ratio of alpha when plante are displayed
        /// </summary>
        private int ratio_alpha_plante = 255; 

        /// <summary>
        /// number of years of the world
        /// </summary>
        public int Years { get; set; } = 1;

        /// <summary>
        /// saison courante du monde
        /// </summary>
        public SeasonTournament SaisonCourante { get; set; }

        /// <summary>
        /// taille du monde en pixel
        /// </summary>
        public Vector2 TaillePx { get; set; }

        /// <summary>
        /// individus du monde
        /// </summary>
        public List<Individu> Individus
        {
            get;
            set;
        }

        /// <summary>
        /// plantes du monde
        /// </summary>
        public List<Plante> Plantes { get; set; }

        /// <summary>
        /// list des fruits
        /// </summary>
        public List<Fruit> Fruits { get; set; }

        /// <summary>
        /// list des graines
        /// </summary>
        public List<Graine> Graines { get; set; }

        /// <summary>
        /// list of eggs
        /// </summary>
        public List<Egg> Eggs { get; set; }

        /// <summary>
        /// parcelle de terrain du monde
        /// </summary>
        public ParcelleTerrain[][] Parcelles
        {
            get;
            set;
        }

        /// <summary>
        /// form to display data
        /// </summary>
        public Form1 DataViewer { get; set; }

        #region IConsoleCommand

        public string Name
        {
            get
            {
                return "World";
            }
        }

        public string Execute(string[] arguments)
        {
            if(arguments != null &&
                arguments.Length > 0)
            {
                switch (arguments[0])
                {
                    case ("Generate"):
                        this.CreateWorld(this.Parcelles.Length, this.Parcelles[0].Length);
                        return "World generated";
                    case ("View"):
                        if(arguments.Length > 1)
                        {
                            switch (arguments[1])
                            {
                                case ("Temperature"):
                                    _view = ViewType.TEMPERATURE;
                                    break;
                                case ("Altitude"):
                                    _view = ViewType.ALTITUDE;
                                    break;
                                case ("Engrais"):
                                    _view = ViewType.ENGRAIS;
                                    break;
                                default:
                                    _view = ViewType.NORMAL;
                                    break;
                            }

                            return "view changed";
                        }
                        break;
                    case ("Tree"):
                        if (arguments.Length > 1 && arguments[1] == "Disable")
                            _viewPlante = false;
                        else if (arguments.Length > 1 && arguments[1] == "Ratio")
                        {
                            if (arguments.Length > 2 && int.TryParse(arguments[2], out ratio_alpha_plante) == false)
                            {
                                ratio_alpha_plante = 255;
                            }
                        }
                        else
                            _viewPlante = true;
                        return "Tree view changed";
                    default:
                        break;
                }
            }

            return "unknow command";
        }

        public string Description
        {
            get
            {
                return "-Generate" + Environment.NewLine +
                        "-View [Temperature-Altitude-Engrais]" + Environment.NewLine +
                        "Tree [Enable;Disable;Ratio[0;255]]";
            }
        }

        #endregion

        public Monde()
        {
            //this.CreateWorld(sizeX, sizeY);
        }

        /// <summary>
        /// return the list of parcelle which represents the perimeter of a square
        /// </summary>
        /// <param name="pos">position of the square (parcelle index)</param>
        /// <param name="radius">radius of the square</param>
        /// <returns>list of parcelle</returns>
        public List<ParcelleTerrain> GetParcellePerimeter(Vector2 pos, int radius)
        {
            List<ParcelleTerrain> list = new List<ParcelleTerrain>();
            for (int i = (int)pos.X - radius; i <= (int)pos.X + radius; i++)
            {
                int j = (int)(pos.Y - radius);
                if(i >= 0 && i < this.Parcelles.Length &&
                    j >= 0 && j < this.Parcelles[i].Length)
                {
                    list.Add(this.Parcelles[i][j]);
                }
            }
            for (int i = (int)pos.X - radius; i <= (int)pos.X + radius; i++)
            {
                int j = (int)(pos.Y + radius);
                if (i >= 0 && i < this.Parcelles.Length &&
                    j >= 0 && j < this.Parcelles[i].Length)
                {
                    list.Add(this.Parcelles[i][j]);
                }
            }
            for (int j = (int)pos.Y - radius+1; j <= (int)pos.Y + radius-1; j++)
            {
                int i = (int)(pos.X + radius);
                if (i >= 0 && i < this.Parcelles.Length &&
                    j >= 0 && j < this.Parcelles[i].Length)
                {
                    list.Add(this.Parcelles[i][j]);
                }
            }
            for (int j = (int)pos.Y - radius + 1; j <= (int)pos.Y + radius - 1; j++)
            {
                int i = (int)(pos.X - radius);
                if (i >= 0 && i < this.Parcelles.Length &&
                    j >= 0 && j < this.Parcelles[i].Length)
                {
                    list.Add(this.Parcelles[i][j]);
                }
            }
            return list;
        }

        public void CreateWorld(int sizex, int sizey)
        {
            if (DataViewer != null)
                DataViewer.ResetChart();

            _timeSaison = 10;
            this.SaisonCourante = SeasonTournament.SAISON1;
            Parcelles = Factory.GenerateGround(sizex, sizey);
            this.TaillePx = new Vector2(sizex * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, sizey * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX);

            this.TemperatureMax = this.Parcelles[0][0].Temperature;
            this.TemperatureMin = this.Parcelles[0][0].Temperature;
            this.AltitudeMax = this.Parcelles[0][0].Altitude;
            this.AltitudeMin = this.Parcelles[0][0].Altitude;

            for (int i = 0; i < this.Parcelles.Length; i++)
            {
                for (int j = 0; j < this.Parcelles[i].Length; j++)
                {
                    if (this.Parcelles[i][j].Altitude > this.AltitudeMax)
                    {
                        this.AltitudeMax = this.Parcelles[i][j].Altitude;
                    }

                    if (this.Parcelles[i][j].Altitude < this.AltitudeMin)
                    {
                        this.AltitudeMin = this.Parcelles[i][j].Altitude;
                    }

                    if (this.Parcelles[i][j].Temperature_Saison1 > this.TemperatureMax)
                    {
                        this.TemperatureMax = this.Parcelles[i][j].Temperature_Saison1;
                    }

                    if (this.Parcelles[i][j].Temperature_Saison1 < this.TemperatureMin)
                    {
                        this.TemperatureMin = this.Parcelles[i][j].Temperature_Saison1;
                    }

                    if (this.Parcelles[i][j].Temperature_Saison2 > this.TemperatureMax)
                    {
                        this.TemperatureMax = this.Parcelles[i][j].Temperature_Saison2;
                    }

                    if (this.Parcelles[i][j].Temperature_Saison2 < this.TemperatureMin)
                    {
                        this.TemperatureMin = this.Parcelles[i][j].Temperature_Saison2;
                    }
                }
            }

            Plantes = new List<Plante>();
            Fruits = new List<Fruit>();
            Individus = new List<Individu>();
            Eggs = new List<Egg>();
            Graines = new List<Graine>();

            Plantes = Factory.AddPlantes(this, 50);
            Fruits = Factory.AddFruits(this, 150);
            Individus = Factory.AddIndividus(this, 12);
            Eggs = Factory.AddEggs(this, 25);
            this.Graines = new List<Graine>();

            for (int i = 0; i < 100; i++)
            {
                Task tParcelle = Task.Factory.StartNew(() => {
                    foreach (var item in this.Parcelles)
                    {
                        Parallel.ForEach(item, x => x.UpdateAsynch((float)rand.NextDouble(), this));
                    }
                });

                Task tPlante = Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(Plantes, x => x.UpdateAsynch((float)rand.NextDouble(), this));
                });
                tParcelle.Wait();
                tPlante.Wait();

                for (int j = this.Plantes.Count - 1; j >= 0; j--)
                {
                    if (this.Plantes[j].Mort == true)
                    {
                        this.Plantes.RemoveAt(j);
                    }
                    else
                    {
                        this.Plantes[j].UpdateSynch((float)rand.NextDouble(), this);
                    }
                }
            }

            Plantes.AddRange(Factory.AddPlantes(this, 5));
        }

        internal void LoadContent(ContentManager content)
        {
            ParcelleTerrain.ParcelleHerbe = content.Load<Texture2D>("Images//herbe");
            ParcelleTerrain.ParcelleNeige = content.Load<Texture2D>("Images//neige");
            ParcelleTerrain.ParcelleDesert = content.Load<Texture2D>("Images//desert");
            ParcelleTerrain.ParcelleEauPeuProfonde = content.Load<Texture2D>("Images//eaupeuprofonde");
            ParcelleTerrain.ParcelleEauProfonde = content.Load<Texture2D>("Images//eauprofonde");
            ParcelleTerrain.Blanc = content.Load<Texture2D>("Images//blanc");

            Plante.Plante1 = content.Load<Texture2D>("Images//plante");
            Plante.Plante_Chaude = content.Load<Texture2D>("Images//plantechaud");
            Plante.Plante_Froide = content.Load<Texture2D>("Images//plantefroid");

            Fruit.Pomme = content.Load<Texture2D>("Images//pomme");

            Graine.GraineSol = content.Load<Texture2D>("Images//graine");

            Individu.IndividuTexture = content.Load<Texture2D>("Images//indi");
            Individu.IndividuFroidTexture = content.Load<Texture2D>("Images//indi_froid");
            Individu.IndividuChaudTexture = content.Load<Texture2D>("Images//indi_chaud");

            Egg.EggGround = content.Load<Texture2D>("Images//oeuf");
        }

        private int _moduloParcelle = 0;

        public void Update(float deltaTime)
        {
            if(this.Plantes.Count == 0 || 
                (this.Individus.Count == 0 && this.Eggs.Count == 0))
            {
                this.CreateWorld(this.Parcelles.Length, this.Parcelles[0].Length);
                return;
            }
                

            this._timeSaison -= deltaTime;
            if(_timeSaison <= 0)
            {
                switch (this.SaisonCourante)
                {
                    case SeasonTournament.SAISON1:
                        Years++;
                        this.SaisonCourante = SeasonTournament.SAISON1_2;
                        _timeSaison = TimeInterSeason;
                        break;
                    case SeasonTournament.SAISON2:
                        this.SaisonCourante = SeasonTournament.SAISON2_1;
                        _timeSaison = TimeInterSeason;
                        break;
                    case SeasonTournament.SAISON1_2:
                        this.SaisonCourante = SeasonTournament.SAISON2;
                        _timeSaison = TimeSeason;
                        break;
                    case SeasonTournament.SAISON2_1:
                        this.SaisonCourante = SeasonTournament.SAISON1;
                        _timeSaison = TimeSeason;
                        break;
                    default:
                        break;
                }
                if(this.DataViewer != null)
                {
                    this.DataViewer.AddSample(this.Individus.Count, this.Eggs.Count);
                }
            }
            HandleGlobalWarming(deltaTime);

            Task tParcelle = Task.Factory.StartNew(() => {
                /*for (int i = 0; i < Parcelles.Length; i+=2)
                {
                    if(_moduloParcelle == 0)
                    {
                        Parallel.ForEach(Parcelles[i], x => x.UpdateAsynch(deltaTime*2, this));
                    }
                    else
                    {
                        Parallel.ForEach(Parcelles[i+1], x => x.UpdateAsynch(deltaTime*2, this));
                    }
                }
                if (_moduloParcelle == 0)
                    _moduloParcelle++;
                else
                    _moduloParcelle = 0;*/
                for (int i = 0; i < this.Parcelles.Length; i++)
                {
                    Parallel.ForEach(Parcelles[i], x => x.UpdateAsynch(deltaTime, this));
                }
            });

            Task tIndividu = Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Individus, x => x.UpdateAsynch(deltaTime, this));
            });

            Task tPlante = Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Plantes, x => x.UpdateAsynch(deltaTime, this));
            });

            Task tFruits = Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Fruits, x => x.UpdateAsynch(deltaTime, this));
            });

            Task tGraine = Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Graines, x => x.UpdateAsynch(deltaTime, this));
            });

            Task tEggs = Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Eggs, x => x.UpdateAsynch(deltaTime, this));
            });

            tFruits.Wait();
            tGraine.Wait();
            tParcelle.Wait();
            tPlante.Wait();
            tIndividu.Wait();
            tEggs.Wait();

            for (int i = this.Plantes.Count - 1; i >= 0; i--)
            {
                if(this.Plantes[i].Mort == true)
                {
                    this.Plantes.RemoveAt(i);
                }
                else
                {
                    this.Plantes[i].UpdateSynch(deltaTime, this);
                }
            }

            for (int i = this.Fruits.Count - 1; i >= 0; i--)
            {
                if (this.Fruits[i].Mort == true)
                {
                    if(this.Fruits[i].Plante != null && rand.Next(0,101) < this.Fruits[i].LuckGraine)
                    {
                        this.Graines.Add(new Graine(this.Fruits[i].Plante, this.Fruits[i].Position, this));
                    }
                    
                    this.Fruits.RemoveAt(i);
                }
            }

            
            for (int i = this.Graines.Count - 1; i >= 0; i--)
            {
                if (this.Graines[i].Mort == true)
                {
                    this.Plantes.Add(this.Graines[i].RefPlante);
                    this.Graines.RemoveAt(i);
                }
            }

            for (int i = this.Individus.Count - 1; i >= 0; i--)
            {
                if(this.Individus[i].Mort == true)
                {
                    this.Individus.RemoveAt(i);
                }
                else
                {
                    this.Individus[i].UpdateSynch(deltaTime, this);
                }
            }

            for (int i = this.Eggs.Count - 1; i >= 0; i--)
            {
                if(this.Eggs[i].Mort == true)
                {
                    this.Individus.Add(this.Eggs[i].RefIndividu);
                    this.Eggs.RemoveAt(i);
                }
            }
        }

        private float _newTemperatureOffset = 0;
        private float _oldTemperatureOffset = 0;
        private int _yearStartWarming = 0;
        public bool GlobalWarmingAction = false;

        private void HandleGlobalWarming(float deltaTime)
        {
            if(this.Years % 5 == 0 && GlobalWarmingAction == false)
            {
                _yearStartWarming = this.Years;
                GlobalWarmingAction = true;
                _newTemperatureOffset = (float)(rand.Next(-5, 6) + (2*rand.NextDouble()+1));
                _oldTemperatureOffset = ParcelleTerrain.OffsetTemperatureParcelle;
            }
            else if(GlobalWarmingAction == true)
            {
                ParcelleTerrain.OffsetTemperatureParcelle += deltaTime * (_newTemperatureOffset - _oldTemperatureOffset) / Monde.TimeChangementGlobalWarming;
                if(this.Years == _yearStartWarming+1)
                {
                    GlobalWarmingAction = false;
                }
            }
        }

        public void DrawActors(SpriteBatch spriteBatch)
        {
            DrawGraines(spriteBatch);
            DrawFruits(spriteBatch);
            DrawPlante(spriteBatch);
            DrawEggs(spriteBatch);
            DrawIndividus(spriteBatch);

        }

        private void DrawIndividus(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.Individus.Count; i++)
            {
                spriteBatch.Draw(this.Individus[i].PictureUsed,
                                this.Individus[i].PositionImage, null,this.Individus[i].Coloration,(float) (this.Individus[i].Angle),
                                new Vector2(this.Individus[i].PictureUsed.Width/2, this.Individus[i].PictureUsed.Height/2),
                                Math.Max(0.1f, this.Individus[i].FactorAgrandissement *2 ),
                                   SpriteEffects.None, 0f);
            }
        }

        private void DrawGraines(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.Graines.Count; i++)
            {
                spriteBatch.Draw(Graine.GraineSol, this.Graines[i].PositionImage, null, Color.White, 0f, Vector2.Zero, this.Graines[i].FactorAgrandissement,
                                   SpriteEffects.None, 0f);
            }
        }

        private void DrawPlante(SpriteBatch spriteBatch)
        {
            float depth;
            if (_viewPlante == false)
                return;
            if(this._view == ViewType.TEMPERATURE)
            {
                byte ratio;
                for (int i = 0; i < this.Plantes.Count; i++)
                {
                    if(this.Plantes[i].Position.Y == this.TaillePx.Y)
                    {
                        depth = this.Plantes[i].Position.Y + i / this.TaillePx.Y;
                    }
                    else
                    {
                        depth = (this.Plantes[i].Position.Y) / (this.TaillePx.Y);
                    }
                    if (this.Plantes[i].TemperatureIdeal < 0)
                    {
                        ratio = (byte)Math.Min(255,
                            (this.Plantes[i].TemperatureIdeal * 255 / this.TemperatureMin));

                        spriteBatch.Draw(this.Plantes[i].PictureUsed, this.Plantes[i].PositionImage, null, new Color(0, 0, ratio), 0f, Vector2.Zero, this.Plantes[i].FactorAgrandissement/2,
                                      SpriteEffects.None, depth);
                    }
                    else
                    {
                        ratio = (byte)Math.Min(255,
                            (this.Plantes[i].TemperatureIdeal * 255 / this.TemperatureMax));

                        spriteBatch.Draw(this.Plantes[i].PictureUsed, this.Plantes[i].PositionImage, null, new Color(ratio, 0,0), 0f, Vector2.Zero, this.Plantes[i].FactorAgrandissement,
                                      SpriteEffects.None, depth);
                    }
                }

            }
            else
            {
                for (int i = 0; i < this.Plantes.Count; i++)
                {
                    if (this.Plantes[i].Position.Y == this.TaillePx.Y)
                    {
                        depth = this.Plantes[i].Position.Y + i / this.TaillePx.Y;
                    }
                    else
                    {
                        depth = (this.Plantes[i].Position.Y) / (this.TaillePx.Y);
                    }
                    spriteBatch.Draw(this.Plantes[i].PictureUsed, this.Plantes[i].PositionImage, null, new Color(255,255,255, ratio_alpha_plante) , 0f, 
                                        Vector2.Zero, this.Plantes[i].FactorAgrandissement,SpriteEffects.None, depth);
                }
            }
        }

        private void DrawFruits(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.Fruits.Count; i++)
            {
                spriteBatch.Draw(Fruit.Pomme, this.Fruits[i].PositionImage, null, Color.White, 0f, Vector2.Zero, this.Fruits[i].FactorAgrandissement,
                                   SpriteEffects.None, 0f);
            }
        }

        private void DrawEggs(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.Eggs.Count; i++)
            {
                spriteBatch.Draw(Egg.EggGround, this.Eggs[i].PositionImage, null, 
                                this.Eggs[i].RefIndividu.Coloration, 0f, Vector2.Zero, this.Eggs[i].FactorAgrandissement,
                                   SpriteEffects.None, 0f);
            }
        }

        public void DrawGround(SpriteBatch spriteBatch)
        {
            byte ratio;
            for (int i = 0; i < this.Parcelles.Length; i++)
            {
                for (int j = 0; j < Parcelles[i].Length; j++)
                {
                    switch (_view)
                    {
                        case ViewType.ENGRAIS:
                            ratio = (byte)(Parcelles[i][j].Engrais * 255 / ParcelleTerrain.ENGRAIS_MAX);
                            spriteBatch.Draw(ParcelleTerrain.Blanc, new Vector2(i * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, j * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX),
                                null, new Color(0, ratio, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            break;
                        case ViewType.ALTITUDE:
                            ratio = (byte)((Parcelles[i][j].Altitude - this.AltitudeMin) * 255 / (this.AltitudeMax - this.AltitudeMin));
                            spriteBatch.Draw(ParcelleTerrain.Blanc, new Vector2(i * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, j * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX),
                                null, new Color(ratio, ratio, ratio), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            break;
                        case ViewType.TEMPERATURE:
                            if (Parcelles[i][j].Temperature < 0)
                            {
                                ratio = (byte)Math.Min(255,
                                    Math.Abs(Parcelles[i][j].Temperature * 255 / this.TemperatureMin));

                                spriteBatch.Draw(ParcelleTerrain.Blanc, new Vector2(i * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, j * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX)
                                    , null, new Color(0, 0, ratio), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            }
                            else
                            {
                                ratio = (byte)Math.Min(255,
                                    Math.Abs(Parcelles[i][j].Temperature * 255 / this.TemperatureMax));

                                spriteBatch.Draw(ParcelleTerrain.Blanc, new Vector2(i * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, j * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX),
                                    null, new Color(ratio, 0, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            }
                            break;
                        case ViewType.NORMAL:
                            if (Parcelles[i][j].Altitude > 0)
                            {
                                if (Parcelles[i][j].Temperature < 0)
                                    spriteBatch.Draw(ParcelleTerrain.ParcelleNeige, new Vector2(i * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, j * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX),
                                        null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                                else if (Parcelles[i][j].Temperature > 30)
                                    spriteBatch.Draw(ParcelleTerrain.ParcelleDesert, new Vector2(i * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, j * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX),
                                        null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                                else
                                    spriteBatch.Draw(ParcelleTerrain.ParcelleHerbe, new Vector2(i * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, j * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX),
                                        null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            }
                            else if (Parcelles[i][j].Altitude < -10)
                            {
                                spriteBatch.Draw(ParcelleTerrain.ParcelleEauProfonde, new Vector2(i * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, j * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX),
                                    null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            }
                            else
                            {
                                spriteBatch.Draw(ParcelleTerrain.ParcelleEauPeuProfonde, new Vector2(i * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX, j * ParcelleTerrain.TAILLE_IMAGE_PARCELLE_PX),
                                    null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}