using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;



namespace ElsiNohitMod.Content
{
    /*
    public class Keybindthingy: ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Keybindings.ReforgeKeybind.JustPressed)
            {
                if (ModContent.GetInstance<ReforgeUI>().reforgeInterface?.CurrentState != null)
                {
                    ModContent.GetInstance<ReforgeUI>().HideUI();
                }
                else
                {
                    ModContent.GetInstance<ReforgeUI>().ShowUI();
                }

                for (int i = 0; i != 58; i++)
                {
                    Item slot = Player.inventory[i];
                    PrefixCategory[] slotCat = slot.GetPrefixCategories().ToArray();
                    if (slotCat.Length != 0)
                    {
                        GetPrefixes(slot);
                    }
                }
            }
        }

        private int[] GetPrefixes(Item item)
        {
            int[] prefixes = Array.Empty<int>();

            for (int i = 1; i <= PrefixLoader.PrefixCount; i++)
            {
                if (item.CanApplyPrefix(i))
                {
                    prefixes = prefixes.Append(i).ToArray();
                }
            }

            return prefixes;
        }
    }
    */

    /*
    class ReforgeUI : ModSystem
    {
        internal UserInterface reforgeInterface;
        internal ReforgeUIState uiState;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                reforgeInterface = new UserInterface();
                uiState = new ReforgeUIState();
                uiState.Activate();
            }
        }

        public override void Unload()
        {
            uiState = null;
        }



        private GameTime _updateTime;

        // Calls for this interface to update
        public override void UpdateUI(GameTime gameTime)
        {
            _updateTime = gameTime;
            if (reforgeInterface?.CurrentState != null)
            {
                reforgeInterface.Update(gameTime);
            }
        }

        // Inserts it within draw layers
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "ElsiNohitMod: reforgeInterface",
                    delegate
                    {
                        if (_updateTime != null && reforgeInterface?.CurrentState != null)
                        {
                            reforgeInterface.Draw(Main.spriteBatch, _updateTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        internal void ShowUI()
        {
            reforgeInterface?.SetState(uiState);
        }

        internal void HideUI()
        {
            reforgeInterface?.SetState(null);
        }
    }
    */ /*
    class ReforgeUIState : UIState
    {
        private static Asset<Texture2D> SelectReforge;
        private static Asset<Texture2D> NextItem;
        private static Asset<Texture2D> ReforgeAll;

        UIText hoverText;

        public override void OnInitialize()
        {
            SelectReforge = ModContent.Request<Texture2D>("ElsiNohitMod/Assets/Reforge");
            NextItem = ModContent.Request<Texture2D>("ElsiNohitMod/Assets/Reforge");
            ReforgeAll = ModContent.Request<Texture2D>("ElsiNohitMod/Assets/Power_Menu_Duplication_Tools_(mobile)");



            hoverText = new UIText("");
            Append(hoverText);

            DraggeablePanel panel = new DraggeablePanel();
            panel.Width.Set(500, 0);
            panel.Height.Set(400, 0);
            panel.SetPadding(10);
            Append(panel);

            UIText header = new UIText("Dynamic Settings");
            header.HAlign = 0.5f;
            header.Top.Set(10, 0);
            panel.Append(header);

            UIPanel scrollArea = new UIPanel();
            scrollArea.VAlign = 1f;
            scrollArea.Width.Set(0, 1f);
            scrollArea.Height.Set(0, 0.9f);
            scrollArea.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            panel.Append(scrollArea);

            UIList list = new UIList();
            list.Width.Set(0f, 1f);
            list.Height.Set(0, 1f);
            list.ListPadding = 5f;


            UIScrollbar scrollBar = new UIScrollbar();
            scrollBar.SetView(100f, 1000f);
            scrollBar.Height.Set(-10f, 1f);
            scrollBar.HAlign = 1f;
            scrollBar.VAlign = 0.5f;
            list.SetScrollbar(scrollBar);

            string[] buttonText =
            {
                "Reforge Menu",
                "Set Spawnpoint",
                "Clear Arrays",
                "Control Time",
                "Control Weather"
            };

            UIImageButton[] buttons =
            [
                new UIImageButton(SelectReforge),
                new UIImageButton(NextItem),
                new UIImageButton(ReforgeAll),
            ];

            UIPanel button = new UIPanel();
            button.Width.Set(0f, 1f);
            button.Height.Set(70f, 0f);
            buttons[0].HAlign = 0.3f;
            buttons[0].VAlign = 0.5f;
            buttons[0].OnMouseOver += HoverReforge;
            button.Append(buttons[0]);
            buttons[1].HAlign = 0.6f;
            buttons[1].VAlign = 0.5f;
            buttons[1].OnMouseOver += HoverNext;
            button.Append(buttons[1]);
            buttons[2].HAlign = 0.9f;
            buttons[2].VAlign = 0.5f;
            buttons[2].OnMouseOver += HoverMassReforge;
            button.Append(buttons[2]);
            list.Add(button);
            /*for (int i = 0; i < buttonText.Length; i++)
            {
                UIPanel button = new UIPanel();
                button.BackgroundColor = new Color(33, 43, 79);
                button.Width = list.Width;
                button.Height.Set(70, 0);
                list.Add(button);
            }*/ /*

            scrollArea.Append(list);
            if (scrollBar.CanScroll)
            {
                list.Width.Set(-12.5f, 1f);
                scrollArea.Append(scrollBar);
            }
        }

        // Changes panel color
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (Elements[0] is UIText hover)
            { // doesn't work
                hover.HAlign = (Main.MouseScreen.X - Left.Pixels)/Width.Pixels;
                hover.VAlign = (Main.MouseScreen.Y - Top.Pixels)/Height.Pixels;
            }
            if (Elements[1] is DraggeablePanel panel)
            {
                panel.BackgroundColor = TheConfigForThisMod.Instance.WindowColor;
            }
            base.DrawChildren(spriteBatch);
        }

        // Hover texts
        private void HoverReforge(UIMouseEvent evt, UIElement element)
        {
            hoverText.SetText("Reforge selected item");
        }
        private void HoverNext(UIMouseEvent evt, UIElement element)
        {
            hoverText.SetText("Swap in next reforgeable item");
        }
        private void HoverMassReforge(UIMouseEvent evt, UIElement element)
        {
            hoverText.SetText("Reforge every possible item in inventory");
        }
    }
    */ /*
    internal class DraggeablePanel : UIPanel
    {
        public Vector2 offset;
        public bool dragging;

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            if (evt.Target == this)
            {
                offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
                dragging = true;
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);

            if (dragging)
            {
                dragging = false;
                Left.Set(evt.MousePosition.X - offset.X, 0f);
                Top.Set(evt.MousePosition.Y - offset.Y, 0f);
                Recalculate();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f);
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            Rectangle parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Contains(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                Recalculate();
            }
        }
    }
    */
}
