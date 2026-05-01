using Gtk;
using System.Reflection.Emit;
using Label = Gtk.Label;

namespace Oblique
{
    internal class Program
    {
        public static IISAProvider isa = new BaseISA();
        public static byte[] Memory = [];
        public static Thread EngineThread;
        static volatile bool running = false;
        public static bool IsRunning { get => running; set { running = value; RunningChanged?.DynamicInvoke(); } }
        public static Window window;

        public static Delegate RunningChanged;

        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("GSETTINGS_SCHEMA_DIR", ".");
            Application.Init();

            window = new Window("Oblique emulator - bslash 1.5");
            window.SetDefaultSize(800,600);

            CreateWidgets();

            window.DeleteEvent += (_, _) =>
            {
                IsRunning = false;
                
                if (EngineThread != null) EngineThread.Join(500);

                Application.Quit();
            };

            window.ShowAll();

            if (args.Length > 0) StartEngine(args[0]);

            Application.Run();
        }

        static void CreateWidgets()
        {
            var box = new Box(Orientation.Vertical, 0);

            var topbar = new MenuBar();
            var bottombar = new MenuBar();

            var screen = new EventBox();
            var evimg = new Image();

            screen.Add(evimg);

            var cssProvider = new CssProvider();
            cssProvider.LoadFromData("* { background-color: black; }");
            screen.StyleContext.AddProvider(cssProvider, 800);

            screen.ShowAll();

            // File menu
            {
                var fileMenu = new Menu();
                var fileItem = new MenuItem("File");
                fileItem.Submenu = fileMenu;

                var loadItem = new MenuItem("Load");
                loadItem.Activated += (_, _) =>
                {
                    var dialog = new FileChooserDialog("Select a file to load", window, FileChooserAction.Open,
                        "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

                    if (dialog.Run() == (int)ResponseType.Accept)
                        StartEngine(dialog.Filename);

                    dialog.Destroy();
                };
                fileMenu.Append(loadItem);

                topbar.Append(fileItem);
            }

            // View menu
            {
                var fileMenu = new Menu();
                var fileItem = new MenuItem("View");
                fileItem.Submenu = fileMenu;

                var loadItem = new MenuItem("Register");
                loadItem.Activated += (_, _) => Register.DumpRegister();
                fileMenu.Append(loadItem);

                topbar.Append(fileItem);
            }

            // Status indicator
            {
                var fileItem = new MenuItem();
                var label = new Label();
                label.Markup = "<span foreground='#FF0000'>Stopped</span>";
                fileItem.Add(label);
                label.Show();

                RunningChanged = () =>
                {
                    string color = IsRunning ? "green" : "red";
                    string text = IsRunning ? "Running" : "Stopped";
                    label.Markup = $"<span foreground='{color}'>{text}</span>";
                };

                bottombar.Append(fileItem);
            }

            box.PackStart(topbar,false,false,0);
            box.PackStart(screen,true,true,0);
            box.PackEnd(bottombar,false,false,0);

            window.Add(box);
        }

        static void StartEngine(string file)
        {
            if (EngineThread != null) EngineThread.Join(500);

            Memory = File.ReadAllBytes(file);
            Register.ResetRegisters();

            IsRunning = true;

            EngineThread = new Thread(EngineLoop)
            {
                IsBackground = true,
                Name = "ObliqueEngine"
            };

            EngineThread.Start();
        }

        static void EngineLoop()
        {
            try
            {
                while (IsRunning)
                {
                    if (Register.IP < Memory.Length) InstructionIvoke();
                    else IsRunning = false;
                }
            }
            catch (Exception ex)
            {
                Application.Invoke((_, _) =>
                {
                    var dialog = new MessageDialog(window, DialogFlags.Modal,
                            MessageType.Error, ButtonsType.Ok, ex.Message);
                    dialog.Run();
                    dialog.Destroy();
                });
            }
        }

        public static void InstructionIvoke()
        {
            var op = Memory[Register.IP];

            if (!isa.InstructionMap.ContainsKey(op)) throw new EmulationException($"Invalid opcode {op:X2} at address {Register.IP:X8}");

            List<object> Parameters = new();

            uint bitsize = 8;

            foreach (var item in isa.InstructionMap[op].Method.GetParameters())
                Parameters.Add(TypeInferer.InferParameter(item.ParameterType,ref bitsize));

            isa.InstructionMap[op].DynamicInvoke(Parameters.ToArray());

            Register.IP += bitsize / 8;
        }
    }
}
