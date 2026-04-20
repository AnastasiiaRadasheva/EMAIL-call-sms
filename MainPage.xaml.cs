using Microsoft.Maui.Controls.Shapes;

namespace _6osaaa
{
    public partial class MainPage : ContentPage
    {
        // ── Поля формы ──────────────────────────────────────────────
        Entry? fookusEntry;
        EntryCell? nimeCell;
        EntryCell? emailCell;
        EntryCell? telefonCell;
        EntryCell? kirjeldusCell;
        EntryCell? sõnumCell;
        Image? ringFoto;
        Label? tervitusLabel;
        string valitudTervitus = "";
        string fotoPath = "";
        int laaditavKontaktIndex = -1;
        Button? uuendaBtn;

        readonly List<string> tervitused = new()
        {
            " Palju õnne sünnipäevaks! Soovin sulle kõike paremat!",
 " Häid jõule ja õnnelikku uut aastat!",
 " Tere kevad! Loodan, et sul on suurepärane päev!",
 " Häid puhkusepäevi! Naudi puhkust täiel rinnal!",
 " Palju õnne lõpetamise puhul! Suur saavutus!",
 " Jätka nii! Sa oled parim sõber!",
 " Sa oled täiesti eriline inimene! "
 };

        VerticalStackLayout? kontaktideNimekiri;

        // ── Teemanvärvid ─────────────────────────────────────────────
        static readonly Color Primaarne = Color.FromArgb("#5B4FE9"); // indigo
        static readonly Color Sekundaarne = Color.FromArgb("#9B8FF5"); // lavendel
        static readonly Color Accent = Color.FromArgb("#F5A623"); // kuldne
        static readonly Color Taust = Color.FromArgb("#F4F3FF"); // väga hele lilla
        static readonly Color Kaart = Colors.White;
        static readonly Color Tekst = Color.FromArgb("#1A1035");
        static readonly Color TekstHall = Color.FromArgb("#7B7497");

        // ════════════════════════════════════════════════════════════
        public MainPage()
        {
            Title = "Sõbrade kontaktiraamat";
            BackgroundColor = Taust;
            EhitaLeht();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300),
                () => fookusEntry?.Focus());
        }

        // ════════════════════════════════════════════════════════════
        //  LEHT
        // ════════════════════════════════════════════════════════════
        void EhitaLeht()
        {
            fookusEntry = new Entry { IsVisible = false, WidthRequest = 1, HeightRequest = 1 };

            var sisukord = new VerticalStackLayout
            {
                Spacing = 0,
                Children =
                {
                    EhitaHero(),
                    EhitaAndmedSektsioon(),
                    EhitaSõnumSektsioon(),
                    EhitaTervitusSektsioon(),
                    EhitaSalvestaNupp(),
                    EhitaNimekirjaLabel(),
                    EhitaNimekiri()
                }
            };

            Content = new ScrollView
            {
                BackgroundColor = Taust,
                Content = new Grid { Children = { sisukord, fookusEntry } }
            };

            UuendaNimekiri();
        }

        // ── HERO (foto + fotopäis) ───────────────────────────────────
        View EhitaHero()
        {
            // Gradient taustaks
            var gradient = new BoxView
            {
                Background = new LinearGradientBrush(
                    new GradientStopCollection
                    {
                        new GradientStop(Primaarne,   0f),
                        new GradientStop(Sekundaarne, 1f)
                    },
                    new Point(0, 0), new Point(1, 1))
            };

            ringFoto = new Image
            {
                Source = ImageSource.FromFile("dotnet_bot.png"),
                WidthRequest = 110,
                HeightRequest = 110,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center,
                Clip = new EllipseGeometry
                { Center = new Point(55, 55), RadiusX = 55, RadiusY = 55 }
            };

            // Белый круг под фото (shadow effect)
            var ringTaust = new Ellipse
            {
                WidthRequest = 118,
                HeightRequest = 118,
                Fill = new SolidColorBrush(Colors.White),
                HorizontalOptions = LayoutOptions.Center
            };

            var heading = new Label
            {
                Text = "Sõbrade kontaktiraamat",
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center
            };

            var subheading = new Label
            {
                Text = "Halda oma sõpru ühes kohas",
                FontSize = 13,
                TextColor = Color.FromArgb("#DDD8FF"),
                HorizontalOptions = LayoutOptions.Center
            };

            var fotoNupud = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10,
                Children =
                {
                    TeeGhostNupp("Teen foto", TeeFoto_Clicked),
                    TeeGhostNupp("Galeriist", ValiGaleriist_Clicked)
                }
            };

            var fotoBlokk = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                HorizontalOptions = LayoutOptions.Center
            };
            fotoBlokk.Add(ringTaust, 0, 0);
            fotoBlokk.Add(ringFoto, 0, 0);

            var content = new VerticalStackLayout
            {
                Spacing = 10,
                Padding = new Thickness(20, 40, 20, 28),
                Children = { heading, subheading, fotoBlokk, fotoNupud }
            };

            return new Grid
            {
                HeightRequest = 300,
                Children = { gradient, content }
            };
        }

        // ── ANDMED ────────────────────────────────────────────────────
        View EhitaAndmedSektsioon()
        {
            nimeCell = UusEntryCell("Nimi", "Sisesta sõbra nimi", Keyboard.Default);
            emailCell = UusEntryCell("Email", "sober@email.com", Keyboard.Email);
            telefonCell = UusEntryCell("Telefon", "+372 5XX XXXX", Keyboard.Telephone);
            kirjeldusCell = UusEntryCell("Kirjeldus", "Kirjeldus sõbra kohta", Keyboard.Default);

            return EhitaKaart("Sõbra andmed", new TableSection
 {
                nimeCell, emailCell, telefonCell, kirjeldusCell
            });
        }

        // ── SÕNUM + TOIMINGUD ─────────────────────────────────────────
        View EhitaSõnumSektsioon()
        {
            sõnumCell = UusEntryCell("Sõnum", "Kirjuta sõnum siia", Keyboard.Default);

            var nupuRida = new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Padding = new Thickness(14, 12),
                    Spacing = 8,
                    Children =
                    {
                        TeePrimaarne("Helista", Helista_Clicked),
                        TeePrimaarne("SMS", SaadaSms_Clicked),
                        TeePrimaarne("Email", SaadaEmail_Clicked)
                    }
                }
            };

            return EhitaKaart("Saada sõnum", new TableSection
 {
                sõnumCell, nupuRida
            });
        }

        // ── TERVITUSED ────────────────────────────────────────────────
        View EhitaTervitusSektsioon()
        {
            tervitusLabel = new Label
            {
                Text = "Vajuta nuppu, et valida juhuslik tervitus",
                FontSize = 13,
                TextColor = TekstHall,
                LineBreakMode = LineBreakMode.WordWrap,
                Margin = new Thickness(0, 0, 0, 8)
            };

            var picker = new Picker
            {
                Title = "Vali tervitus...",
                HorizontalOptions = LayoutOptions.Fill,
                TextColor = Tekst
            };
            foreach (var t in tervitused) picker.Items.Add(t);
            picker.SelectedIndexChanged += (s, e) =>
            {
                if (picker.SelectedIndex >= 0)
                {
                    valitudTervitus = tervitused[picker.SelectedIndex];
                    if (tervitusLabel != null)
                        tervitusLabel.Text = $"Valitud: {valitudTervitus}";
                }
            };

            var rndNupp = TeePrimaarne("Juhuslik tervitus", ValiTervitus_Clicked);
            rndNupp.HorizontalOptions = LayoutOptions.Fill;

            var tervitusValikCell = new ViewCell
            {
                View = new VerticalStackLayout
                {
                    Padding = new Thickness(14, 12),
                    Spacing = 8,
                    Children = { tervitusLabel, picker, rndNupp }
                }
            };

            var saatmiseNupud = new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Padding = new Thickness(14, 10),
                    Spacing = 8,
                    Children =
                    {
                        TeeAccentNupp("SMS tervitus", SaadaTervitusSms_Clicked),
                        TeeAccentNupp("Email tervitus", SaadaTervitusEmail_Clicked)
 }
                }
            };

            return EhitaKaart("Juhuslik tervitus", new TableSection
 {
                tervitusValikCell, saatmiseNupud
            });
        }

        // ── SALVESTA ──────────────────────────────────────────────────
        View EhitaSalvestaNupp()
        {
            // "Salvesta muudatused" — aktiivne ainult kui kontakt on nimekirjast laaditud
            uuendaBtn = new Button
            {
                Text = "Salvesta muudatused",
                BackgroundColor = Color.FromArgb("#94A3B8"), // hall = mitteaktiivne
                TextColor = Colors.White,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 16,
                Padding = new Thickness(0, 14),
                Margin = new Thickness(20, 8, 20, 4),
                IsEnabled = false
            };
            uuendaBtn.Clicked += Salvesta_Clicked;

            // "Uus kontakt" — loob alati uue sloti, ignoreerib laaditavKontaktIndex
            var uusBtn = new Button
            {
                Text = "Uus kontakt",
                BackgroundColor = Color.FromArgb("#22C55E"),
                TextColor = Colors.White,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 16,
                Padding = new Thickness(0, 14),
                Margin = new Thickness(20, 4, 20, 4),
                Shadow = new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#22C55E")), Offset = new Point(0, 4), Radius = 10, Opacity = 0.4f }
            };
            uusBtn.Clicked += SalvestaUus_Clicked;

            var tyhjendaBtn = new Button
            {
                Text = "Puhasta väljad",
                BackgroundColor = Color.FromArgb("#EF4444"),
                TextColor = Colors.White,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 16,
                Padding = new Thickness(0, 14),
                Margin = new Thickness(20, 4, 20, 8),
                Shadow = new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#EF4444")), Offset = new Point(0, 4), Radius = 10, Opacity = 0.4f }
            };
            tyhjendaBtn.Clicked += (s, e) => TyhjendaVorm();

            return new VerticalStackLayout { Children = { uuendaBtn, uusBtn, tyhjendaBtn } };
        }

        Label EhitaNimekirjaLabel()
        {
            return new Label
            {
                Text = "Salvestatud kontaktid",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Tekst,
                Margin = new Thickness(20, 20, 20, 4)
            };
        }

        VerticalStackLayout EhitaNimekiri()
        {
            kontaktideNimekiri = new VerticalStackLayout
            {
                Padding = new Thickness(16, 4, 16, 24),
                Spacing = 10
            };
            return kontaktideNimekiri;
        }

        // ════════════════════════════════════════════════════════════
        //  SALVESTA / KUSTUTA / LAADI
        // ════════════════════════════════════════════════════════════
        // Uuendab olemasolevat kontakti (aktiivne ainult kui kontakt on laaditud)
        async void Salvesta_Clicked(object? sender, EventArgs e)
        {
            if (laaditavKontaktIndex < 0)
            {
                await DisplayAlertAsync("Viga", "Ükski kontakt pole valitud!", "OK");
                return;
            }
            string nimi = nimeCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(nimi))
            {
                await DisplayAlertAsync("Viga", "Sisesta vähemalt nimi!", "OK");
                return;
            }
            SalvestaIndeksile(laaditavKontaktIndex);
            TyhjendaVorm();
            UuendaNimekiri();
            await DisplayAlertAsync("Uuendatud", "Kontakt on uuendatud!", "OK");
        }

        // Loob alati uue kontakti (ignoreerib laaditud kontakti)
        async void SalvestaUus_Clicked(object? sender, EventArgs e)
        {
            string nimi = nimeCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(nimi))
            {
                await DisplayAlertAsync("Viga", "Sisesta vähemalt nimi!", "OK");
                return;
            }
            int index = LeidaVabaSlot();
            if (index < 0)
            {
                await DisplayAlertAsync("Viga", "Kontaktide limit on 5!", "OK");
                return;
            }
            SalvestaIndeksile(index);
            TyhjendaVorm();
            UuendaNimekiri();
            await DisplayAlertAsync("Salvestatud", "Uus kontakt lisatud!", "OK");
        }

        void SalvestaIndeksile(int index)
        {
            Preferences.Set($"k{index}_nimi", nimeCell?.Text ?? "");
            Preferences.Set($"k{index}_email", emailCell?.Text ?? "");
            Preferences.Set($"k{index}_tel", telefonCell?.Text ?? "");
            Preferences.Set($"k{index}_kirj", kirjeldusCell?.Text ?? "");
            Preferences.Set($"k{index}_foto", fotoPath);
            Preferences.Set($"k{index}_on", true);
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
            fotoPath = "";
            laaditavKontaktIndex = -1;
            if (ringFoto != null) ringFoto.Source = ImageSource.FromFile("dotnet_bot.png");
            if (uuendaBtn != null)
            {
                uuendaBtn.IsEnabled = false;
                uuendaBtn.BackgroundColor = Color.FromArgb("#94A3B8");
                uuendaBtn.Shadow = null;
            }
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
                string nimi = Preferences.Get($"k{i}_nimi", "");
                string email = Preferences.Get($"k{i}_email", "");
                string tel = Preferences.Get($"k{i}_tel", "");
                string foto = Preferences.Get($"k{i}_foto", "");
                int idx = i;

                var img = new Image
                {
                    WidthRequest = 52,
                    HeightRequest = 52,
                    Aspect = Aspect.AspectFill,
                    Clip = new EllipseGeometry { Center = new Point(26, 26), RadiusX = 26, RadiusY = 26 }
                };
                img.Source = !string.IsNullOrEmpty(foto) && File.Exists(foto)
                    ? ImageSource.FromFile(foto)
                    : ImageSource.FromFile("dotnet_bot.png");

                var nimeLbl = new Label { Text = nimi, FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Tekst };
                var emailLbl = new Label { Text = email, FontSize = 12, TextColor = TekstHall };
                var telLbl = new Label { Text = tel, FontSize = 12, TextColor = TekstHall };

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

                var info = new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 1,
                    Children = { nimeLbl, emailLbl, telLbl }
                };

                var rida = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = 60 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 44 }
                    },
                    Padding = new Thickness(12),
                    BackgroundColor = Kaart
                };
                rida.Add(img, 0, 0);
                rida.Add(info, 1, 0);
                rida.Add(kustutaBtn, 2, 0);

                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => LaadKontakt(idx);
                rida.GestureRecognizers.Add(tap);

                var kaart = new Border
                {
                    Content = rida,
                    BackgroundColor = Kaart,
                    StrokeShape = new RoundRectangle { CornerRadius = 16 },
                    Stroke = Color.FromArgb("#E5E1FF"),
                    StrokeThickness = 1,
                    Shadow = new Shadow
                    {
                        Brush = new SolidColorBrush(Color.FromArgb("#5B4FE9")),
                        Offset = new Point(0, 2),
                        Radius = 8,
                        Opacity = 0.08f
                    }
                };

                kontaktideNimekiri.Children.Add(kaart);
            }

            if (!keegi)
            {
                kontaktideNimekiri.Children.Add(new Label
                {
                    Text = "Kontakte pole veel lisatud.\nVäljasta vormi ja vajuta Salvesta.",
                    TextColor = TekstHall,
                    FontSize = 13,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10)
                });
            }
        }

        void LaadKontakt(int i)
        {
            laaditavKontaktIndex = i;
            if (uuendaBtn != null)
            {
                uuendaBtn.IsEnabled = true;
                uuendaBtn.BackgroundColor = Color.FromArgb("#5B4FE9");
                uuendaBtn.Shadow = new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#5B4FE9")), Offset = new Point(0, 4), Radius = 10, Opacity = 0.4f };
            }
            if (nimeCell != null) nimeCell.Text = Preferences.Get($"k{i}_nimi", "");
            if (emailCell != null) emailCell.Text = Preferences.Get($"k{i}_email", "");
            if (telefonCell != null) telefonCell.Text = Preferences.Get($"k{i}_tel", "");
            if (kirjeldusCell != null) kirjeldusCell.Text = Preferences.Get($"k{i}_kirj", "");
            fotoPath = Preferences.Get($"k{i}_foto", "");
            if (ringFoto != null)
                ringFoto.Source = !string.IsNullOrEmpty(fotoPath) && File.Exists(fotoPath)
                    ? ImageSource.FromFile(fotoPath)
                    : ImageSource.FromFile("dotnet_bot.png");
        }

        void KustutaKontakt(int i)
        {
            Preferences.Remove($"k{i}_nimi");
            Preferences.Remove($"k{i}_email");
            Preferences.Remove($"k{i}_tel");
            Preferences.Remove($"k{i}_kirj");
            Preferences.Remove($"k{i}_foto");
            Preferences.Set($"k{i}_on", false);
            if (laaditavKontaktIndex == i) TyhjendaVorm();
            UuendaNimekiri();
        }

        // ════════════════════════════════════════════════════════════
        //  FOTO
        // ════════════════════════════════════════════════════════════
        async void TeeFoto_Clicked(object? sender, EventArgs e)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await DisplayAlertAsync("Info", "Kaamera pole toetatud. Kasuta galeriid.", "OK");
                    return;
                }

                FileResult? photo = null;
                try
                {
                    photo = await MediaPicker.Default.CapturePhotoAsync();
                }
                catch (OperationCanceledException)
                {
                    return; // kasutaja sulges — OK
                }
                catch
                {
                    await DisplayAlertAsync("Info", "Kaamera ei tööta emulaatoril. Kasuta \'Galeriist\'.", "OK");
                    return;
                }

                if (photo == null) return;

                var destPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using (var src = await photo.OpenReadAsync())
                using (var dst = File.OpenWrite(destPath))
                    await src.CopyToAsync(dst);

                fotoPath = destPath;
                if (ringFoto != null)
                    ringFoto.Source = ImageSource.FromFile(fotoPath);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Viga", ex.Message, "OK");
            }
        }

        async void ValiGaleriist_Clicked(object? sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo == null) return;
                fotoPath = photo.FullPath;
                if (ringFoto != null) ringFoto.Source = ImageSource.FromFile(fotoPath);
            }
            catch (Exception ex) { await DisplayAlertAsync("Viga", ex.Message, "OK"); }
        }

        // ════════════════════════════════════════════════════════════
        //  HELISTA / SMS / EMAIL
        // ════════════════════════════════════════════════════════════
        async void Helista_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone))
            { await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK"); return; }
            if (PhoneDialer.Default.IsSupported) PhoneDialer.Default.Open(phone);
            else await DisplayAlertAsync("Viga", "Helistamine pole toetatud", "OK");
        }

        async void SaadaSms_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonCell?.Text ?? "";
            string msg = sõnumCell?.Text ?? "Tere!";
            if (string.IsNullOrWhiteSpace(phone))
            { await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK"); return; }
            if (Sms.Default.IsComposeSupported)
                await Sms.Default.ComposeAsync(new SmsMessage(msg, phone));
            else await DisplayAlertAsync("Viga", "SMS pole toetatud", "OK");
        }

        async void SaadaEmail_Clicked(object? sender, EventArgs e)
        {
            string email = emailCell?.Text ?? "";
            string nimi = nimeCell?.Text ?? "Sõber";
            string msg = sõnumCell?.Text ?? "Tere!";
            string kirjeldus = kirjeldusCell?.Text ?? "";

            if (string.IsNullOrWhiteSpace(email))
            { await DisplayAlertAsync("Viga", "Sisesta emailiaadress!", "OK"); return; }

            string body = string.IsNullOrWhiteSpace(kirjeldus)
                           ? msg
                           : $"{msg}\n\n---\nKirjeldus: {kirjeldus}";

            var mail = new EmailMessage
            {
                Subject = $"Sõnum sõbrale {nimi}",
                Body = body,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string> { email }
            };
            if (Email.Default.IsComposeSupported) await Email.Default.ComposeAsync(mail);
            else await DisplayAlertAsync("Viga", "Email pole toetatud", "OK");
        }

        // ════════════════════════════════════════════════════════════
        //  TERVITUSED
        // ════════════════════════════════════════════════════════════
        void ValiTervitus_Clicked(object? sender, EventArgs e)
        {
            valitudTervitus = tervitused[new Random().Next(tervitused.Count)];
            if (tervitusLabel != null) tervitusLabel.Text = $"Valitud: {valitudTervitus}";
        }

        async void SaadaTervitusSms_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus))
            { await DisplayAlertAsync("Viga", "Vali esmalt tervitus!", "OK"); return; }
            string phone = telefonCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone))
            { await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK"); return; }
            if (Sms.Default.IsComposeSupported)
                await Sms.Default.ComposeAsync(new SmsMessage(valitudTervitus, phone));
            else await DisplayAlertAsync("Viga", "SMS pole toetatud", "OK");
        }

        async void SaadaTervitusEmail_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus))
            { await DisplayAlertAsync("Viga", "Vali esmalt tervitus!", "OK"); return; }
            string email = emailCell?.Text ?? "";
            string nimi = nimeCell?.Text ?? "Sõber";
            if (string.IsNullOrWhiteSpace(email))
            { await DisplayAlertAsync("Viga", "Sisesta emailiaadress!", "OK"); return; }
            var mail = new EmailMessage
            {
                Subject = $" Tervitus sõbrale {nimi}",
                Body = valitudTervitus,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string> { email }
            };
            if (Email.Default.IsComposeSupported) await Email.Default.ComposeAsync(mail);
            else await DisplayAlertAsync("Viga", "Email pole toetatud", "OK");
        }

        // ════════════════════════════════════════════════════════════
        //  ABImeetodid (UI factory)
        // ════════════════════════════════════════════════════════════

        /// Mähib TableSection valge ümardatud kaarti
        View EhitaKaart(string pealkiri, TableSection sektsioon)
        {
            var label = new Label
            {
                Text = pealkiri,
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextColor = Primaarne,
                Margin = new Thickness(20, 18, 20, 4)
            };

            var tabel = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot { sektsioon },
                BackgroundColor = Colors.Transparent,
                HasUnevenRows = true
            };

            var border = new Border
            {
                Content = tabel,
                BackgroundColor = Kaart,
                StrokeShape = new RoundRectangle { CornerRadius = 20 },
                Stroke = Color.FromArgb("#E5E1FF"),
                StrokeThickness = 1,
                Margin = new Thickness(16, 0, 16, 0),
                Shadow = new Shadow
                {
                    Brush = new SolidColorBrush(Color.FromArgb("#5B4FE9")),
                    Offset = new Point(0, 3),
                    Radius = 10,
                    Opacity = 0.07f
                }
            };

            return new VerticalStackLayout { Children = { label, border } };
        }

        EntryCell UusEntryCell(string label, string placeholder, Keyboard keyboard)
        {
            return new EntryCell
            {
                Label = label,
                Placeholder = placeholder,
                Keyboard = keyboard
            };
        }

        Button TeePrimaarne(string tekst, EventHandler handler)
        {
            var btn = new Button
            {
                Text = tekst,
                BackgroundColor = Primaarne,
                TextColor = Colors.White,
                CornerRadius = 14,
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(12, 9),
                Shadow = new Shadow
                {
                    Brush = new SolidColorBrush(Primaarne),
                    Offset = new Point(0, 3),
                    Radius = 8,
                    Opacity = 0.35f
                }
            };
            btn.Clicked += handler;
            return btn;
        }

        Button TeeAccentNupp(string tekst, EventHandler handler)
        {
            var btn = new Button
            {
                Text = tekst,
                BackgroundColor = Accent,
                TextColor = Colors.White,
                CornerRadius = 14,
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(12, 9)
            };
            btn.Clicked += handler;
            return btn;
        }

        Button TeeGhostNupp(string tekst, EventHandler handler)
        {
            var btn = new Button
            {
                Text = tekst,
                BackgroundColor = Color.FromArgb("#2A1D80"),
                TextColor = Colors.White,
                CornerRadius = 20,
                FontSize = 12,
                Padding = new Thickness(14, 8),
                BorderColor = Color.FromArgb("#7B72F0"),
                BorderWidth = 1
            };
            btn.Clicked += handler;
            return btn;
        }

        // Совместимый хелпер для DisplayAlert
        Task DisplayAlertAsync(string title, string message, string cancel)
            => DisplayAlert(title, message, cancel);
    }
}