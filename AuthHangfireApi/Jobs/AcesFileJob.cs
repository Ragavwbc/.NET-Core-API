using System.Text;
using System.Xml.Linq;
using Hangfire;

namespace AuthHangfireApi.Jobs
{
    public class AcesFileJob
    {
        private static readonly object _lock = new();

        [DisableConcurrentExecution(300)]
        public void GenerateAcesFile()
        {
            lock (_lock)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, "ACES_LIVE.xml");

                XDocument doc;

                // 1️⃣ Create base file if not exists
                if (!File.Exists(filePath))
                {
                    doc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement("ACES",
                            new XAttribute("version", "4.2"),
                            new XElement("Header",
                                new XElement("Company", "Automotive industry"),
                                new XElement("SenderName", "msaharia@masterdatamigration.com"),
                                new XElement("TransferDate", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                                new XElement("BrandAAIAID", "HBGS"),
                                new XElement("DocumentTitle", "LIVE ACES FILE"),
                                new XElement("SubmissionType", "FULL")
                            ),
                            new XElement("Footer",
                                new XElement("RecordCount", 0)
                            )
                        )
                    );
                }
                else
                {
                    doc = XDocument.Load(filePath);
                }

                var aces = doc.Root!;
                var footer = aces.Element("Footer")!;
                var recordCount = int.Parse(footer.Element("RecordCount")!.Value);

                // 2️⃣ UNIQUE + HIGHLY VISIBLE APP NODE
                var now = DateTime.Now;

                var app = new XElement("App",
                    new XAttribute("action", "A"),
                    new XAttribute("id", Guid.NewGuid()),
                    new XElement("GeneratedAt", now.ToString("yyyy-MM-dd HH:mm:ss")), // ⭐ HIGHLIGHT
                    new XElement("BaseVehicle", new XAttribute("id", "28007")),
                    new XElement("SubModel", new XAttribute("id", "2002")),
                    new XElement("Qty", 3),
                    new XElement("PartType", new XAttribute("id", "1684")),
                    new XElement("Position", new XAttribute("id", "22")),
                    new XElement("Part", "1551237101")
                );

                // Insert before footer
                footer.AddBeforeSelf(app);

                // Update count
                footer.Element("RecordCount")!.Value = (recordCount + 1).ToString();

                doc.Save(filePath, SaveOptions.DisableFormatting);

                Console.WriteLine($"✅ App added at {now}");
            }
        }


        [DisableConcurrentExecution(120)] // 2 mins safety
        public void AddAppTag()
        {
            lock (_lock)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, "ACES_LIVE.xml");

                XDocument doc;

                // Create file if missing
                if (!File.Exists(filePath))
                {
                    doc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement("ACES",
                            new XAttribute("version", "4.2"),
                            new XElement("Header",
                                new XElement("Company", "Automotive industry"),
                                new XElement("SenderName", "msaharia@masterdatamigration.com"),
                                new XElement("TransferDate", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                                new XElement("SubmissionType", "FULL")
                            ),
                            new XElement("Footer",
                                new XElement("RecordCount", 0)
                            )
                        )
                    );
                }
                else
                {
                    doc = XDocument.Load(filePath);
                }

                var root = doc.Root!;
                var footer = root.Element("Footer")!;
                var count = int.Parse(footer.Element("RecordCount")!.Value);

                var now = DateTime.Now;

                var app = new XElement("App",
                    new XAttribute("action", "A"),
                    new XAttribute("id", Guid.NewGuid()),
                    new XElement("GeneratedAt", now.ToString("yyyy-MM-dd HH:mm:ss")), // ⭐ visible
                    new XElement("BaseVehicle", new XAttribute("id", "28007")),
                    new XElement("Qty", 1),
                    new XElement("Part", "1551237101")
                );

                footer.AddBeforeSelf(app);
                footer.Element("RecordCount")!.Value = (count + 1).ToString();

                doc.Save(filePath, SaveOptions.DisableFormatting);

                Console.WriteLine($"🧩 App added at {now}");
            }
        }
    }
}
