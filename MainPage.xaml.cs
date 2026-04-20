using Microsoft.Maui.Controls.Shapes;

namespace _6osaaa
{
    public partial class MainPage : ContentPage
    {
        EntryCell? nimeCell, emailCell, telefonCell, kirjeldusCell, sõnumCell;
        Image? ringFoto;
        Label? tervitusLabel;
        Button? uuendaBtn;
        VerticalStackLayout? kontaktideNimekiri;

        string valitudTervitus = "";
        string fotoPath = "";
        int laaditavKontaktIndex = -1;

        readonly List<string> tervitused = new()
        {
            "Palju õnne sünnipäevaks! Soovin sulle kõike paremat!",
            "Häid jõule ja õnnelikku uut aastat!",
            "Tere kevad! Loodan, et sul on suurepärane päev!",
            "Häid puhkusepäevi! Naudi puhkust täiel rinnal!",
            "Palju õnne lõpetamise puhul! Suur saavutus!",
            "Jätka nii! Sa oled parim sõber!",
            "Sa oled täiesti eriline inimene!"
        };
          
        static readonly Color Primaarne = Color.FromArgb("#5B4FE9");
        static readonly Color Accent = Color.FromArgb("#F5A623");
        static readonly Color Taust = Color.FromArgb("#F4F3FF");
        static readonly Color Tekst = Color.FromArgb("#1A1035");
        static readonly Color TekstHall = Color.FromArgb("#7B7497");

        public MainPage()
        {
            Title = "Sõbrade kontaktiraamat";
            BackgroundColor = Taust;
            EhitaLeht();
        }

        void EhitaLeht()
        {
            kontaktideNimekiri = new VerticalStackLayout { Padding = new Thickness(16, 4, 16, 24), Spacing = 10 };

            Content = new ScrollView
            {
                BackgroundColor = Taust,
                Content = new VerticalStackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        EhitaHero(),
                        EhitaAndmedSektsioon(),
                        EhitaSõnumSektsioon(),
                        EhitaTervitusSektsioon(),
                        EhitaNupud(),
                        new Label { Text = "Salvestatud kontaktid", FontSize = 18, FontAttributes = FontAttributes.Bold,
                                    TextColor = Tekst, Margin = new Thickness(20, 20, 20, 4) },
                        kontaktideNimekiri
                    }
                }
            };

            UuendaNimekiri();
        }

        View EhitaHero()
        {
            ringFoto = new Image
            {
                Source = "dotnet_bot.png",
                WidthRequest = 110,
                HeightRequest = 110,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center,
                Clip = new EllipseGeometry { Center = new Point(55, 55), RadiusX = 55, RadiusY = 55 }
            };

            var fotoBlokk = new Grid { HorizontalOptions = LayoutOptions.Center };
            fotoBlokk.Add(new Ellipse
            {
                WidthRequest = 118,
                HeightRequest = 118,
                Fill = new SolidColorBrush(Colors.White),
                HorizontalOptions = LayoutOptions.Center
            });
            fotoBlokk.Add(ringFoto);

            var gradient = new BoxView
            {
                Background = new LinearGradientBrush(
                    new GradientStopCollection { new(Color.FromArgb("#5B4FE9"), 0f), new(Color.FromArgb("#9B8FF5"), 1f) },
                    new Point(0, 0), new Point(1, 1))
            };

            var content = new VerticalStackLayout
            {
                Spacing = 10,
                Padding = new Thickness(20, 40, 20, 28),
                Children =
                {
                    new Label { Text = "Sõbrade kontaktiraamat", FontSize = 22, FontAttributes = FontAttributes.Bold,
                                TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center },
                    new Label { Text = "Halda oma sõpru ühes kohas", FontSize = 13,
                                TextColor = Color.FromArgb("#DDD8FF"), HorizontalOptions = LayoutOptions.Center },
                    fotoBlokk,
                    new HorizontalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.Center, Spacing = 10,
                        Children = { Nupp("Teen foto", "#2A1D80", TeeFoto_Clicked, ghost: true),
                                     Nupp("Galeriist", "#2A1D80", ValiGaleriist_Clicked, ghost: true) }
                    }
                }
            };

            return new Grid { HeightRequest = 300, Children = { gradient, content } };
        }

        // ── Andmed ───────────────────────────────────────────────────
        View EhitaAndmedSektsioon()
        {
            nimeCell = new EntryCell { Label = "Nimi", Placeholder = "Sisesta sõbra nimi" };
            emailCell = new EntryCell { Label = "Email", Placeholder = "sober@email.com", Keyboard = Keyboard.Email };
            telefonCell = new EntryCell { Label = "Telefon", Placeholder = "+372 5XX XXXX", Keyboard = Keyboard.Telephone };
            kirjeldusCell = new EntryCell { Label = "Kirjeldus", Placeholder = "Kirjeldus sõbra kohta" };
            return Kaart("Sobra andmed", new TableSection { nimeCell, emailCell, telefonCell, kirjeldusCell });
        }

        // ── Sonum ────────────────────────────────────────────────────
        View EhitaSõnumSektsioon()
        {
            sõnumCell = new EntryCell { Label = "Sonum", Placeholder = "Kirjuta sonum siia" };
            var nupud = new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Padding = new Thickness(14, 12),
                    Spacing = 8,
                    Children =
                    {
                        Nupp("Helista", "#5B4FE9", Helista_Clicked),
                        Nupp("SMS",     "#5B4FE9", SaadaSms_Clicked),
                        Nupp("Email",   "#5B4FE9", SaadaEmail_Clicked)
                    }
                }
            };
            return Kaart("Saada sonum", new TableSection { sõnumCell, nupud });
        }

        // ── Tervitused ───────────────────────────────────────────────
        View EhitaTervitusSektsioon()
        {
            tervitusLabel = new Label
            {
                Text = "Vajuta nuppu, et valida juhuslik tervitus",
                FontSize = 13,
                TextColor = TekstHall,
                LineBreakMode = LineBreakMode.WordWrap
            };

            var picker = new Picker { Title = "Vali tervitus...", HorizontalOptions = LayoutOptions.Fill, TextColor = Tekst };
            foreach (var t in tervitused) picker.Items.Add(t);
            picker.SelectedIndexChanged += (s, e) =>
            {
                if (picker.SelectedIndex >= 0)
                {
                    valitudTervitus = tervitused[picker.SelectedIndex];
                    tervitusLabel.Text = "Valitud: " + valitudTervitus;
                }
            };

            var juhuslikNupp = Nupp("Juhuslik tervitus", "#5B4FE9", ValiTervitus_Clicked);
            juhuslikNupp.HorizontalOptions = LayoutOptions.Fill;

            var valikCell = new ViewCell
            {
                View = new VerticalStackLayout
                {
                    Padding = new Thickness(14, 12),
                    Spacing = 8,
                    Children = { tervitusLabel, picker, juhuslikNupp }
                }
            };
            var saatmineCell = new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Padding = new Thickness(14, 10),
                    Spacing = 8,
                    Children =
                    {
                        Nupp("SMS tervitus",   "#F5A623", SaadaTervitusSms_Clicked),
                        Nupp("Email tervitus", "#F5A623", SaadaTervitusEmail_Clicked)
                    }
                }
            };
            return Kaart("Juhuslik tervitus", new TableSection { valikCell, saatmineCell });
        }

        // ── Kolm nuppu ───────────────────────────────────────────────
        View EhitaNupud()
        {
            uuendaBtn = Nupp("Salvesta muudatused", "#94A3B8", Salvesta_Clicked);
            uuendaBtn.Margin = new Thickness(20, 8, 20, 4);
            uuendaBtn.IsEnabled = false;

            var uusBtn = Nupp("Uus kontakt", "#22C55E", SalvestaUus_Clicked);
            uusBtn.Margin = new Thickness(20, 4, 20, 4);

            var puhastaBnt = Nupp("Puhasta väljad", "#EF4444", (s, e) => TyhjendaVorm());
            puhastaBnt.Margin = new Thickness(20, 4, 20, 8);

            return new VerticalStackLayout { Children = { uuendaBtn, uusBtn, puhastaBnt } };
        }

        // ── Salvesta / Kustuta / Laadi ────────────────────────────────
        async void Salvesta_Clicked(object? sender, EventArgs e)
        {
            if (laaditavKontaktIndex < 0) { await Alert("Viga", "Ukski kontakt pole valitud!"); return; }
            if (string.IsNullOrWhiteSpace(nimeCell?.Text)) { await Alert("Viga", "Sisesta vahemalt nimi!"); return; }
            SalvestaIndeksile(laaditavKontaktIndex);
            TyhjendaVorm(); UuendaNimekiri();
            await Alert("Uuendatud", "Kontakt on uuendatud!");
        }

        async void SalvestaUus_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nimeCell?.Text)) { await Alert("Viga", "Sisesta vahemalt nimi!"); return; }
            int index = LeidaVabaSlot();
            if (index < 0) { await Alert("Viga", "Kontaktide limit on 5!"); return; }
            SalvestaIndeksile(index);
            TyhjendaVorm(); UuendaNimekiri();
            await Alert("Salvestatud", "Uus kontakt lisatud!");
        }

        void SalvestaIndeksile(int i)
        {
            Preferences.Set($"k{i}_nimi", nimeCell?.Text ?? "");
            Preferences.Set($"k{i}_email", emailCell?.Text ?? "");
            Preferences.Set($"k{i}_tel", telefonCell?.Text ?? "");
            Preferences.Set($"k{i}_kirj", kirjeldusCell?.Text ?? "");
            Preferences.Set($"k{i}_foto", fotoPath);
            Preferences.Set($"k{i}_on", true);
        }

        int LeidaVabaSlot()
        {
            for (int i = 0; i < 5; i++)
                if (!Preferences.Get($"k{i}_on", false)) return i;
            return -1;
        }

        void TyhjendaVorm()
        {
            if (nimeCell != null) nimeCell.Text = "";
            if (emailCell != null) emailCell.Text = "";
            if (telefonCell != null) telefonCell.Text = "";
            if (kirjeldusCell != null) kirjeldusCell.Text = "";
            if (sõnumCell != null) sõnumCell.Text = "";
            fotoPath = ""; laaditavKontaktIndex = -1;
            if (ringFoto != null) ringFoto.Source = "dotnet_bot.png";
            if (uuendaBtn != null) { uuendaBtn.IsEnabled = false; uuendaBtn.BackgroundColor = Color.FromArgb("#94A3B8"); uuendaBtn.Shadow = null; }
        }

        void UuendaNimekiri()
        {
            if (kontaktideNimekiri == null) return;
            kontaktideNimekiri.Children.Clear();

            bool keegi = false;
            for (int i = 0; i < 5; i++)
            {
                if (!Preferences.Get($"k{i}_on", false)) continue;
                keegi = true;
                int idx = i;

                var img = new Image
                {
                    WidthRequest = 52,
                    HeightRequest = 52,
                    Aspect = Aspect.AspectFill,
                    Clip = new EllipseGeometry { Center = new Point(26, 26), RadiusX = 26, RadiusY = 26 }
                };
                string foto = Preferences.Get($"k{i}_foto", "");
                img.Source = !string.IsNullOrEmpty(foto) && File.Exists(foto) ? foto : "dotnet_bot.png";

                var kustutaBtn = new Button
                {
                    Text = "X",
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Red,
                    FontSize = 20,
                    Padding = 0,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                };
                kustutaBtn.Clicked += (s, e) => KustutaKontakt(idx);

                var rida = new Grid
                {
                    ColumnDefinitions = { new(60), new(GridLength.Star), new(44) },
                    Padding = new Thickness(12),
                    BackgroundColor = Colors.White
                };
                rida.Add(img, 0, 0);
                rida.Add(new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 1,
                    Children =
                    {
                        new Label { Text = Preferences.Get($"k{i}_nimi",  ""), FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Tekst },
                        new Label { Text = Preferences.Get($"k{i}_email", ""), FontSize = 12, TextColor = TekstHall },
                        new Label { Text = Preferences.Get($"k{i}_tel",   ""), FontSize = 12, TextColor = TekstHall }
                    }
                }, 1, 0);
                rida.Add(kustutaBtn, 2, 0);

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => LaadKontakt(idx);
                rida.GestureRecognizers.Add(tap);

                kontaktideNimekiri.Children.Add(new Border
                {
                    Content = rida,
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    Stroke = Color.FromArgb("#E5E1FF"),
                    StrokeThickness = 1
                });
            }

            if (!keegi)
                kontaktideNimekiri.Children.Add(new Label
                {
                    Text = "Kontakte pole veel lisatud.",
                    TextColor = TekstHall,
                    FontSize = 13,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10)
                });
        }

        void LaadKontakt(int i)
        {
            laaditavKontaktIndex = i;
            if (nimeCell != null) nimeCell.Text = Preferences.Get($"k{i}_nimi", "");
            if (emailCell != null) emailCell.Text = Preferences.Get($"k{i}_email", "");
            if (telefonCell != null) telefonCell.Text = Preferences.Get($"k{i}_tel", "");
            if (kirjeldusCell != null) kirjeldusCell.Text = Preferences.Get($"k{i}_kirj", "");
            fotoPath = Preferences.Get($"k{i}_foto", "");
            if (ringFoto != null) ringFoto.Source = !string.IsNullOrEmpty(fotoPath) && File.Exists(fotoPath) ? fotoPath : "dotnet_bot.png";
            if (uuendaBtn != null)
            {
                uuendaBtn.IsEnabled = true; uuendaBtn.BackgroundColor = Primaarne;
                uuendaBtn.Shadow = new Shadow { Brush = new SolidColorBrush(Primaarne), Offset = new Point(0, 4), Radius = 10, Opacity = 0.4f };
            }
        }

        void KustutaKontakt(int i)
        {
            foreach (var key in new[] { "nimi", "email", "tel", "kirj", "foto" })
                Preferences.Remove($"k{i}_{key}");
            Preferences.Set($"k{i}_on", false);
            if (laaditavKontaktIndex == i) TyhjendaVorm();
            UuendaNimekiri();
        }

        // ── Foto ─────────────────────────────────────────────────────
        async void TeeFoto_Clicked(object? sender, EventArgs e)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported) { await Alert("Info", "Kaamera pole toetatud. Kasuta galeriid."); return; }
                FileResult? photo;
                try { photo = await MediaPicker.Default.CapturePhotoAsync(); }
                catch (OperationCanceledException) { return; }
                catch { await Alert("Info", "Kaamera ei toota emulaatoril. Kasuta 'Galeriist'."); return; }
                if (photo == null) return;
                fotoPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using var src = await photo.OpenReadAsync();
                using var dst = File.OpenWrite(fotoPath);
                await src.CopyToAsync(dst);
                if (ringFoto != null) ringFoto.Source = fotoPath;
            }
            catch (Exception ex) { await Alert("Viga", ex.Message); }
        }

        async void ValiGaleriist_Clicked(object? sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo == null) return;
                fotoPath = photo.FullPath;
                if (ringFoto != null) ringFoto.Source = fotoPath;
            }
            catch (Exception ex) { await Alert("Viga", ex.Message); }
        }

        // ── Helista / SMS / Email ─────────────────────────────────────
        async void Helista_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone)) { await Alert("Viga", "Sisesta telefoninumber!"); return; }
            if (PhoneDialer.Default.IsSupported) PhoneDialer.Default.Open(phone);
            else await Alert("Viga", "Helistamine pole toetatud");
        }

        async void SaadaSms_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone)) { await Alert("Viga", "Sisesta telefoninumber!"); return; }
            if (Sms.Default.IsComposeSupported) await Sms.Default.ComposeAsync(new SmsMessage(sõnumCell?.Text ?? "Tere!", phone));
            else await Alert("Viga", "SMS pole toetatud");
        }

        async void SaadaEmail_Clicked(object? sender, EventArgs e)
        {
            string email = emailCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(email)) { await Alert("Viga", "Sisesta emailiaadress!"); return; }
            string kirjeldus = kirjeldusCell?.Text ?? "";
            string msg = sõnumCell?.Text ?? "Tere!";
            string body = string.IsNullOrWhiteSpace(kirjeldus) ? msg : $"{msg}\n\n---\nKirjeldus: {kirjeldus}";
            var mail = new EmailMessage
            {
                Subject = $"Sonum sobrale {nimeCell?.Text ?? "Sober"}",
                Body = body,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string> { email }
            };
            if (Email.Default.IsComposeSupported) await Email.Default.ComposeAsync(mail);
            else await Alert("Viga", "Email pole toetatud");
        }

        // ── Tervitused ────────────────────────────────────────────────
        void ValiTervitus_Clicked(object? sender, EventArgs e)
        {
            valitudTervitus = tervitused[new Random().Next(tervitused.Count)];
            if (tervitusLabel != null) tervitusLabel.Text = "Valitud: " + valitudTervitus;
        }

        async void SaadaTervitusSms_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus)) { await Alert("Viga", "Vali esmalt tervitus!"); return; }
            string phone = telefonCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone)) { await Alert("Viga", "Sisesta telefoninumber!"); return; }
            if (Sms.Default.IsComposeSupported) await Sms.Default.ComposeAsync(new SmsMessage(valitudTervitus, phone));
            else await Alert("Viga", "SMS pole toetatud");
        }

        async void SaadaTervitusEmail_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus)) { await Alert("Viga", "Vali esmalt tervitus!"); return; }
            string email = emailCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(email)) { await Alert("Viga", "Sisesta emailiaadress!"); return; }
            var mail = new EmailMessage
            {
                Subject = $"Tervitus sobrale {nimeCell?.Text ?? "Sober"}",
                Body = valitudTervitus,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string> { email }
            };
            if (Email.Default.IsComposeSupported) await Email.Default.ComposeAsync(mail);
            else await Alert("Viga", "Email pole toetatud");
        }

        // ── UI abimeetodid ────────────────────────────────────────────
        View Kaart(string pealkiri, TableSection sektsioon) => new VerticalStackLayout
        {
            Children =
            {
                new Label { Text = pealkiri, FontSize = 13, FontAttributes = FontAttributes.Bold,
                            TextColor = Primaarne, Margin = new Thickness(20, 18, 20, 4) },
                new Border
                {
                    Content = new TableView { Intent = TableIntent.Form, Root = new TableRoot { sektsioon },
                                              BackgroundColor = Colors.Transparent, HasUnevenRows = true },
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                    Stroke = Color.FromArgb("#E5E1FF"), StrokeThickness = 1,
                    Margin = new Thickness(16, 0),
                    Shadow = new Shadow { Brush = new SolidColorBrush(Primaarne), Offset = new Point(0, 3), Radius = 10, Opacity = 0.07f }
                }
            }
        };

        Button Nupp(string tekst, string värv, EventHandler handler, bool ghost = false)
        {
            var btn = new Button
            {
                Text = tekst,
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb(värv),
                CornerRadius = ghost ? 20 : 14,
                FontSize = ghost ? 12 : 13,
                FontAttributes = FontAttributes.Bold,
                Padding = ghost ? new Thickness(14, 8) : new Thickness(12, 9)
            };
            if (ghost) { btn.BorderColor = Color.FromArgb("#7B72F0"); btn.BorderWidth = 1; }
            btn.Clicked += handler;
            return btn;
        }

        Task Alert(string title, string msg) => DisplayAlert(title, msg, "OK");
    }
}