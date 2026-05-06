using Gtk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oblique
{
    public class ObliqueDebugger : Window
    {
        List<HexRowWidget> HexRows = new();
        public static uint CurrentAddr = 0xA0000000;

        uint rowHeight = 16;
        uint lastH = 0;
        Box? flowBox;

        public ObliqueDebugger() : base("Oblique Debugger")
        {
            SetDefaultSize(800, 450);
            Resizable = false;

            flowBox = new Box(Orientation.Vertical, 0);
            flowBox.Homogeneous = false;

            RebuildCells(800, 450);

            var eventBox = new EventBox();
            eventBox.AddEvents((int)Gdk.EventMask.ScrollMask);
            eventBox.ScrollEvent += (_, args) =>
            {
                if (args.Event.Direction == Gdk.ScrollDirection.Up)
                {
                    uint maxLookback = (uint)(Program.isa.InstructionSizeMap.Keys.Max() / 8) + 2;

                    int idx = -(int)maxLookback;

                    for (int tries = 0; tries < maxLookback * 2; tries++)
                    {
                        while (!Program.isa.InstructionSizeMap.ContainsKey(Program.Memory[(uint)(CurrentAddr + idx)]))
                            idx++;

                        var (ist,size) = PullInstructionData(idx);

                        if (CurrentAddr + idx + size == CurrentAddr)
                        {
                            CurrentAddr = (uint)(CurrentAddr + idx);
                            break;
                        }
                        else idx ++;
                    }

                    IterateRebuildContent();
                }
                else if (args.Event.Direction == Gdk.ScrollDirection.Down)
                {
                    uint idx = 0;
                    while (!Program.isa.InstructionSizeMap.ContainsKey(Program.Memory[CurrentAddr + idx]))
                        idx++;

                    var op = Program.Memory[CurrentAddr + idx];
                    uint size = (uint)Program.isa.InstructionSizeMap[op] / 8 + 1;
                    CurrentAddr += idx + size;
                    IterateRebuildContent();
                }
            };

            eventBox.Add(flowBox);

            var tb = new Toolbar();
            tb.ToolbarStyle = ToolbarStyle.Icons;

            // Stepping mode
            {
                ToolButton btn = new(Program.SteppedExecution ? Stock.MediaPlay : Stock.MediaPause);
                btn.Clicked += (_, _) =>
                {
                    Program.SteppedExecution = !Program.SteppedExecution;
                    btn.StockId = Program.SteppedExecution ? Stock.MediaPlay : Stock.MediaPause;
                };

                tb.Insert(btn,-1);
            }

            // Step button
            {
                ToolButton btn = new(Stock.MediaForward);
                btn.Clicked += (_, _) => 
                {
                    if (!Program.SteppedExecution)
                    {
                        var dialog = new MessageDialog(Program.window, DialogFlags.Modal,
                            MessageType.Error, ButtonsType.Ok, $"stepped execution is not enabled");
                        dialog.Run();
                        dialog.Destroy();

                        return;
                    }

                    CurrentAddr = Register.IP;

                    var (a, b) = PullInstructionData(0);

                    btn.TooltipText = "Current: "+ Program.isa.InstructionAliases[Program.Memory[a]] + $" (0x{a:X8})";

                    Program.IsPaused = false;

                    while (!Program.IsPaused && Program.IsRunning);

                    IterateRebuildContent();
                };

                tb.Insert(btn, -1);
            }

            var vb = new Box(Orientation.Vertical, 0);
            vb.PackStart(tb,false,false,0);
            vb.PackStart(new Separator(Orientation.Horizontal), false, false, 0);
            vb.PackStart(eventBox,true,true,0);

            Add(vb);

            ShowAll();
        }

        public void IterateRebuildContent()
        {
            uint idx = 0;

            foreach (var item in HexRows)
            {
                var (ist, size) = PullInstructionData((int)idx);
                item.Update(ist, size);
                idx += size;
            }
        }

        (uint,uint) PullInstructionData(int idx)
        {
            while (!Program.isa.InstructionSizeMap.ContainsKey(Program.Memory[(uint)(CurrentAddr + idx)])) idx++;

            uint ist = (uint)(CurrentAddr + idx);
            var op = Program.Memory[ist];

            uint size = (uint)Program.isa.InstructionSizeMap[op] / 8;

            return (ist, size);
        }

        void RebuildCells(uint W, uint H)
        {
            int h = (int)H - (int)lastH;

            int rowcount = Math.Max(0, h / (int)rowHeight);

            uint idx = 0;

            for (uint i = 0; i < rowcount; i++)
            {
                var (ist,size) = PullInstructionData((int)idx);

                var row = new HexRowWidget(ist, size);
                HexRows.Add(row);
                flowBox!.PackStart(row, false, false, 0);
                idx += size;
            }

            flowBox!.ShowAll();
        }
    }

    public class HexRowWidget : Box
    {
        Label addr;
        Label bytes;
        Label instr;

        public HexRowWidget(uint baseAddr, uint count) : base(Orientation.Horizontal, 4)
        {
            addr = new();
            addr.UseMarkup = true;
            addr.Xalign = 0;
            addr.WidthRequest = 90;
            addr.ModifyFont(Pango.FontDescription.FromString("Monospace 10"));

            bytes = new();
            bytes.UseMarkup = true;
            bytes.Xalign = 0;
            bytes.ModifyFont(Pango.FontDescription.FromString("Monospace 10"));

            instr = new();
            instr.UseMarkup = true;
            instr.Xalign = 0;
            instr.ModifyFont(Pango.FontDescription.FromString("Monospace 10"));

            HeightRequest = 16;

            PackStart(addr,false,false,5);
            PackStart(new Separator(Orientation.Horizontal), false, false, 0);
            PackStart(bytes, false, false, 0);
            PackStart(new Separator(Orientation.Horizontal), false, false, 5);
            PackStart(instr, true, true, 0);

            Update(baseAddr,count);
        }

        public void Update(uint baseAddr, uint count)
        {
            addr.Markup = $"<span foreground='#66f'>0x{baseAddr:X8}</span>";

            string sb = "";

            for (int i = 0; i < count; i++)
            {
                byte b = Program.Memory[baseAddr + (uint)i];
                sb += $" {b:X2}";
            }
            sb += new string(' ', (int)(36 - count * 3));

            byte op = Program.Memory[baseAddr];

            bytes.Markup = $"<span foreground='#444'>{sb}</span>";
            instr.Markup = $"<span foreground='#444'>{Program.isa.InstructionAliases[op]}</span>";
        }
    }
}
