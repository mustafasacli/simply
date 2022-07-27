using Simply.Common;
using Simply.Common.Objects;
using Simply.Data;
using Simply.Data.Objects;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace SI.OracleSP.TestApp
{
    internal class Program
    {
        internal static OracleConnection GetConnection()
        {
            return new OracleConnection
            {
                ConnectionString = "********************"
            };
        }

        private static void Main(string[] args)
        {
            //066173301
            List<NobetMemurListesiViewModel> liste = new List<NobetMemurListesiViewModel>();
            Stopwatch stopwatch = new Stopwatch();
            using (var connection = GetConnection())
            {
                try
                {
                    //string spSql = "KYKEDONUSUM.GET_NOBET_MEMUR_LISTE(:PYURTKODU, :PBAS_TARIH, :PBITIS_TARIH, :OUT_CRSR)";
                    string spSql = @"BEGIN
                        KYKEDONUSUM.GET_NOBET_MEMUR_LISTE(:PYURTKODU, :PBAS_TARIH, :PBITIS_TARIH, :OUT_CRSR);
                        END;";
                    DbCommandDefinition commandDefinition = new DbCommandDefinition()
                    {
                        CommandText = spSql,
                        CommandType = System.Data.CommandType.Text
                    };
                    commandDefinition
                        .AddParameterAndReturn(new DbCommandParameter { ParameterName = "PYURTKODU", Value = "066173301", Direction = System.Data.ParameterDirection.Input, ParameterDbType = null })
                        .AddParameterAndReturn(new DbCommandParameter { ParameterName = "PBAS_TARIH", Value = DateTime.Today.AddDays(-200), Direction = System.Data.ParameterDirection.Input })
                        .AddParameterAndReturn(new DbCommandParameter { ParameterName = "PBITIS_TARIH", Value = DateTime.Today.AddDays(-1), Direction = System.Data.ParameterDirection.Input })
                        ;

                    OracleParameter oracleParameter = (OracleParameter)connection.CreateDbCommandParameter(parameterName: ":OUT_CRSR", direction: System.Data.ParameterDirection.Output);
                    oracleParameter.OracleDbType = OracleDbType.RefCursor;
                    commandDefinition.AddDbParameter(oracleParameter);
                    connection.OpenIfNot();
                    stopwatch.Start();
                    liste = connection.GetList<NobetMemurListesiViewModel>(commandDefinition).Result
                        ?? new List<NobetMemurListesiViewModel>();
                    stopwatch.Stop();
                }
                finally
                {
                    connection.CloseIfNot();
                }
            }

            Console.WriteLine("Geçen süre(ms): " + stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Kayıt sayısı: " + (liste?.Count ?? 0));
            if ((liste?.Count ?? 0) > 0)
            {
                Console.WriteLine("Kayıt başı ortalama süre: " + ((double)stopwatch.ElapsedMilliseconds / (double)liste.Count));
            }
            PropertyInfo[] properties = typeof(NobetMemurListesiViewModel).GetValidPropertiesOfTypeV2(includeReadonlyProperties: true);
            foreach (NobetMemurListesiViewModel item in liste)
            {
                foreach (var property in properties)
                {
                    Console.WriteLine(string.Format("Property:{0}, Column:{1}, Value:#{2}#", property.Name, property.GetColumnNameOfProperty(), property.GetValue(item, null)));
                }

                Console.WriteLine("-------------------------------");
            }
            //dynamic dd = SimpleDbRow.NewRow();
            //dd.ALI = "aLİ";
            //string dds = dd.ALI.ToStr();
            Console.ReadKey();
        }
    }

    [DataContract]
    public class NobetMemurListesiViewModel
    {
        [DataMember]
        [Column("ID")]
        public decimal Id
        { get; set; }

        [DataMember]
        [Column("YURTKODU")]
        public string YurtKodu
        { get; set; }

        [DataMember]
        [Column("TARIH")]
        public DateTime? Tarih
        { get; set; }

        [DataMember]
        [Column("NOBETCI_KULLANICIFK")]
        public string NobetciKullaniciFk
        { get; set; }

        [DataMember]
        [Column("ISLEM_KULLANICI")]
        public string IslemKullanici
        { get; set; }

        [DataMember]
        [Column("ISLEM_ZAMAN")]
        public DateTime? IslemZamani
        { get; set; } = new DateTime();

        [DataMember]
        [Column("SILINDI")]
        public decimal? Silindi
        { get; set; } = 0;

        [DataMember]
        [Column("ONAY_KODU")]
        public string OnayKodu
        { get; set; }

        [DataMember]
        [Column("PERSONEL_NOBET_NOT")]
        public string PersonelNobetNot
        { get; set; }

        [DataMember]
        [Column("NOBET_DEFTERI_ID")]
        public string NobetDefteriId
        { get; set; }

        [DataMember]
        [Column("NOBETCI_ID")]
        public string NobetciId
        { get; set; }

        [DataMember]
        [Column("NOBET_TARIH")]
        public string NobetTarihi
        { get; set; }

        [DataMember]
        [Column("AD")]
        public string Ad
        { get; set; }

        [DataMember]
        [Column("SOYAD")]
        public string Soyad
        { get; set; }

        [DataMember]
        [Column("SICIL")]
        public string Sicil
        { get; set; }

        [DataMember]
        [Column("BASLANGIC_SAAT")]
        public string BaslangicSaat
        { get; set; }

        [DataMember]
        [Column("NOBET_DEFTER_ID")]
        public decimal? NobetDefterId
        { get; set; }

        [DataMember]
        [Column("NOBETCI_ISLEM_TARIH")]
        public DateTime? NobetciIslemTarih
        { get; set; }

        [DataMember]
        [Column("YURT_MUDURU_ONAY")]
        public decimal? YurtMuduruOnay
        { get; set; }

        [DataMember]
        [Column("YURT_MUDURU_TARIH")]
        public DateTime? YurtMuduruTarih
        { get; set; }

        [DataMember]
        public string TarihText
        { get { return Tarih.HasValue ? Tarih.Value.ToShortDateString() : string.Empty; } }

        [DataMember]
        public string AdSoyad
        { get { return string.Join(" ", this.Ad, this.Soyad); } }

        [DataMember]
        public bool PersonelImzaladiMi
        { get { return string.IsNullOrWhiteSpace(this.BaslangicSaat) == false && this.NobetciIslemTarih != null; } }

        [DataMember]
        public string TarihSimpleText
        { get { return Tarih.HasValue ? Tarih.Value.ToString("yyyy-MM-dd") : string.Empty; } }

        [DataMember]
        public bool TarihGeldiMi
        {
            get
            {
                var r = this.Tarih.HasValue ?
                    this.Tarih.Value.Date < DateTime.Today
                    : false;

                return r;
            }
        }

        [DataMember]
        public bool MudurImzaladiMi
        { get { return this.YurtMuduruTarih != null && this.YurtMuduruOnay.GetValueOrDefault() > 0; } }
    }
}