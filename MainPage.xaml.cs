using Microsoft.Maui.Controls.Shapes;

namespace _6osaaa
{
    public partial class MainPage
    {
        public MainPage()
        {
            Title = "Sõbrade kontaktiraamat";
            EhitaLeht();
        }

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
        int laaditavKontaktIndex = -1; // какой контакт редактируем (-1 = новый)

        readonly List<string> tervitused = new()
        {
            "🎂 Palju õnne sünnipäevaks! Soovin sulle kõike paremat!",
            "🎄 Häid jõule ja õnnelikku uut aastat!",
            "🌸 Tere kevad! Loodan, et sul on suurepärane päev!",
            "🏖️ Häid puhkusepäevi! Naudi puhkust täiel rinnal!",
            "🎓 Palju õnne lõpetamise puhul! Suur saavutus!",
            "💪 Jätka nii! Sa oled parim sõber!",
            "🌟 Sa oled täiesti eriline inimene! 🌟"
        };

        // Список контактов внизу (обновляется после сохранения)
        VerticalStackLayout? kontaktideNimekiri;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300), () =>
            {
                fookusEntry?.Focus();
            });
        }

        void EhitaLeht()
        {
            // ---- Круглое фото ----
            ringFoto = new Image
            {
                Source = ImageSource.FromFile("dotnet_bot.png"),
                WidthRequest = 100,
                HeightRequest = 100,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Center,
                Clip = new EllipseGeometry { Center = new Point(50, 50), RadiusX = 50, RadiusY = 50 }
            };

            var fotoNupud = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10,
                Children =
                {
                    TeeBtnNupp("📷 Teen foto",  "#6200EE", TeeFoto_Clicked),
                    TeeBtnNupp("🖼️ Valin foto", "#6200EE", ValiGaleriist_Clicked)
                }
            };

            var header = new VerticalStackLayout
            {
                BackgroundColor = Color.FromArgb("#F0F0F0"),
                Padding = new Thickness(0, 20, 0, 16),
                Spacing = 10,
                Children = { ringFoto, fotoNupud }
            };

            // ---- Andmed ----
            nimeCell = new EntryCell { Label = "Nimi", Placeholder = "Sisesta sõbra nimi" };
            emailCell = new EntryCell { Label = "Email", Placeholder = "Sisesta email", Keyboard = Keyboard.Email };
            telefonCell = new EntryCell { Label = "Telefon", Placeholder = "Sisesta tel. number", Keyboard = Keyboard.Telephone };
            kirjeldusCell = new EntryCell { Label = "Kirjeldus", Placeholder = "Kirjeldus sõbra kohta" };

            var andmedSection = new TableSection("👤 Sõbra andmed")
            {
                nimeCell, emailCell, telefonCell, kirjeldusCell
            };

            // ---- Sõnum ----
            sõnumCell = new EntryCell { Label = "Sõnum", Placeholder = "Kirjuta sõnum siia" };

            var nupudView = new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Padding = new Thickness(15, 8),
                    Spacing = 8,
                    Children =
                    {
                        TeeBtnNupp("📞 Helista", "#6200EE", Helista_Clicked),
                        TeeBtnNupp("💬 SMS",     "#6200EE", SaadaSms_Clicked),
                        TeeBtnNupp("📧 Email",   "#6200EE", SaadaEmail_Clicked)
                    }
                }
            };

            var sõnumSection = new TableSection("✉️ Saada sõnum") { sõnumCell, nupudView };

            // ---- Tervitus ----
            tervitusLabel = new Label
            {
                Text = "Vajuta nuppu, et valida juhuslik tervitus",
                FontSize = 13,
                TextColor = Colors.Gray,
                LineBreakMode = LineBreakMode.WordWrap,
                Margin = new Thickness(0, 0, 0, 4)
            };

            var picker = new Picker { Title = "📋 Vali tervitus...", HorizontalOptions = LayoutOptions.Fill };
            foreach (var t in tervitused) picker.Items.Add(t);
            picker.SelectedIndexChanged += (s, e) =>
            {
                if (picker.SelectedIndex >= 0)
                {
                    valitudTervitus = tervitused[picker.SelectedIndex];
                    tervitusLabel.Text = $"✅ {valitudTervitus}";
                }
            };

            var rndNupp = TeeBtnNupp("🎲 Juhuslik", "#03DAC6", ValiTervitus_Clicked);
            rndNupp.HorizontalOptions = LayoutOptions.Fill;

            var tervitusValikView = new ViewCell
            {
                View = new VerticalStackLayout
                {
                    Padding = new Thickness(15, 10),
                    Spacing = 6,
                    Children = { tervitusLabel, picker, rndNupp }
                }
            };
            tervitusValikView.Height = 160;

            var tervitusSaatmineView = new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Padding = new Thickness(15, 8),
                    Spacing = 8,
                    Children =
                    {
                        TeeBtnNupp("📱 Tervitus SMS",   "#018786", SaadaTervitusSms_Clicked),
                        TeeBtnNupp("✉️ Tervitus Email", "#018786", SaadaTervitusEmail_Clicked)
                    }
                }
            };

            var tervitusSection = new TableSection("🎉 Juhuslik tervitus")
            {
                tervitusValikView, tervitusSaatmineView
            };

            // ---- Кнопка Salvesta ----
            var salvestaView = new ViewCell
            {
                View = new VerticalStackLayout
                {
                    Padding = new Thickness(15, 10),
                    Children =
                    {
                        TeeBtnNupp("💾 Salvesta kontakt", "#388E3C", Salvesta_Clicked)
                    }
                }
            };

            var salvestaSection = new TableSection { salvestaView };

            // ---- TableView ----
            var tabelview = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot
                {
                    andmedSection,
                    sõnumSection,
                    tervitusSection,
                    salvestaSection
                }
            };

            // ---- Список контактов внизу ----
            kontaktideNimekiri = new VerticalStackLayout
            {
                Padding = new Thickness(15, 10),
                Spacing = 8
            };

            var nimekirjaLabel = new Label
            {
                Text = "📋 Salvestatud kontaktid",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(15, 10, 0, 0)
            };

            fookusEntry = new Entry { IsVisible = false, WidthRequest = 1, HeightRequest = 1 };

            var page = new VerticalStackLayout
            {
                Children = { header, tabelview, nimekirjaLabel, kontaktideNimekiri }
            };

            Content = new ScrollView
            {
                Content = new Grid { Children = { page, fookusEntry } }
            };

            // Загружаем сохранённые контакты
            UuendaNimekiri();
        }

        // ---- Сохранить контакт ----
        async void Salvesta_Clicked(object? sender, EventArgs e)
        {
            string nimi = nimeCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(nimi))
            {
                await DisplayAlertAsync("Viga", "Sisesta vähemalt nimi!", "OK");
                return;
            }

            // Ищем свободный слот или перезаписываем редактируемый
            int index = laaditavKontaktIndex >= 0 ? laaditavKontaktIndex : LeidaVabaSlot();

            if (index < 0)
            {
                await DisplayAlertAsync("Viga", "Kontaktide limit on 5!", "OK");
                return;
            }

            Preferences.Set($"kontakt_{index}_nimi", nimeCell?.Text ?? "");
            Preferences.Set($"kontakt_{index}_email", emailCell?.Text ?? "");
            Preferences.Set($"kontakt_{index}_telefon", telefonCell?.Text ?? "");
            Preferences.Set($"kontakt_{index}_kirjeldus", kirjeldusCell?.Text ?? "");
            Preferences.Set($"kontakt_{index}_foto", fotoPath);
            Preferences.Set($"kontakt_{index}_on", true);

            TyhjendaVorm();
            UuendaNimekiri();
            await DisplayAlertAsync("OK", "Kontakt salvestatud!", "OK");
        }

        int LeidaVabaSlot()
        {
            for (int i = 0; i < 5; i++)
                if (!Preferences.Get($"kontakt_{i}_on", false))
                    return i;
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
            if (ringFoto != null)
                ringFoto.Source = ImageSource.FromFile("dotnet_bot.png");
        }
 
        void UuendaNimekiri()
        {
            if (kontaktideNimekiri == null) return;
            kontaktideNimekiri.Children.Clear();

            for (int i = 0; i < 5; i++)
            {
                if (!Preferences.Get($"kontakt_{i}_on", false)) continue;

                string nimi = Preferences.Get($"kontakt_{i}_nimi", "");
                string email = Preferences.Get($"kontakt_{i}_email", "");
                string foto = Preferences.Get($"kontakt_{i}_foto", "");
                int idx = i; // захват для лямбды

                // Фото контакта
                var img = new Image
                {
                    WidthRequest = 48,
                    HeightRequest = 48,
                    Aspect = Aspect.AspectFill,
                    Clip = new EllipseGeometry { Center = new Point(24, 24), RadiusX = 24, RadiusY = 24 }
                };
                img.Source = !string.IsNullOrEmpty(foto) && File.Exists(foto)
                    ? ImageSource.FromFile(foto)
                    : ImageSource.FromFile("dotnet_bot.png");

                var nimeTekst = new Label
                {
                    Text = nimi,
                    FontSize = 15,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center
                };

                var emailTekst = new Label
                {
                    Text = email,
                    FontSize = 12,
                    TextColor = Colors.Gray,
                    VerticalOptions = LayoutOptions.Center
                };

                // Кнопка удалить
                var kustutaBtn = new Button
                {
                    Text = "🗑️",
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Red,
                    FontSize = 18,
                    Padding = 0,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                };
                kustutaBtn.Clicked += (s, e) => KustutaKontakt(idx);

                var rida = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = 56 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 40 }
                    },
                    BackgroundColor = Color.FromArgb("#F8F8F8"),
                    Padding = new Thickness(8),
                    Children = { img, new VerticalStackLayout { Children = { nimeTekst, emailTekst } }, kustutaBtn }
                };
                Microsoft.Maui.Controls.Grid.SetColumn((View)img, 0);
                Microsoft.Maui.Controls.Grid.SetColumn((View)rida.Children[1], 1);
                Microsoft.Maui.Controls.Grid.SetColumn((View)kustutaBtn, 2);

                // Нажатие на контакт — загрузить в форму
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => LaadKontakt(idx);
                rida.GestureRecognizers.Add(tap);

                // Граница вокруг строки
                var border = new Border
                {
                    Content = rida,
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Stroke = Color.FromArgb("#E0E0E0"),
                    StrokeThickness = 1
                };

                kontaktideNimekiri.Children.Add(border);
            }
        }

        void LaadKontakt(int i)
        {
            laaditavKontaktIndex = i;
            if (nimeCell != null) nimeCell.Text = Preferences.Get($"kontakt_{i}_nimi", "");
            if (emailCell != null) emailCell.Text = Preferences.Get($"kontakt_{i}_email", "");
            if (telefonCell != null) telefonCell.Text = Preferences.Get($"kontakt_{i}_telefon", "");
            if (kirjeldusCell != null) kirjeldusCell.Text = Preferences.Get($"kontakt_{i}_kirjeldus", "");
            fotoPath = Preferences.Get($"kontakt_{i}_foto", "");

            if (ringFoto != null)
                ringFoto.Source = !string.IsNullOrEmpty(fotoPath) && File.Exists(fotoPath)
                    ? ImageSource.FromFile(fotoPath)
                    : ImageSource.FromFile("dotnet_bot.png");

            // Скролл наверх не нужен — просто форма заполнена
        }

        void KustutaKontakt(int i)
        {
            Preferences.Remove($"kontakt_{i}_nimi");
            Preferences.Remove($"kontakt_{i}_email");
            Preferences.Remove($"kontakt_{i}_telefon");
            Preferences.Remove($"kontakt_{i}_kirjeldus");
            Preferences.Remove($"kontakt_{i}_foto");
            Preferences.Set($"kontakt_{i}_on", false);
            if (laaditavKontaktIndex == i) TyhjendaVorm();
            UuendaNimekiri();
        }

        // ---- Фото ----
        async void TeeFoto_Clicked(object? sender, EventArgs e)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await DisplayAlertAsync("Viga", "Kaamera pole toetatud", "OK");
                    return;
                }
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo == null) return;
                fotoPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using var stream = await photo.OpenReadAsync();
                using var newStream = File.OpenWrite(fotoPath);
                await stream.CopyToAsync(newStream);
                if (ringFoto != null) ringFoto.Source = ImageSource.FromFile(fotoPath);
            }
            catch (Exception ex) { await DisplayAlertAsync("Viga", ex.Message, "OK"); }
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

        // ---- Helista / SMS / Email ----
        async void Helista_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone)) { await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK"); return; }
            if (PhoneDialer.Default.IsSupported) PhoneDialer.Default.Open(phone);
            else await DisplayAlertAsync("Viga", "Helistamine pole toetatud", "OK");
        }

        async void SaadaSms_Clicked(object? sender, EventArgs e)
        {
            string phone = telefonCell?.Text ?? "";
            string msg = sõnumCell?.Text ?? "Tere!";
            if (string.IsNullOrWhiteSpace(phone)) { await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK"); return; }
            if (Sms.Default.IsComposeSupported) await Sms.Default.ComposeAsync(new SmsMessage(msg, phone));
            else await DisplayAlertAsync("Viga", "SMS pole toetatud", "OK");
        }

        async void SaadaEmail_Clicked(object? sender, EventArgs e)
        {
            string email = emailCell?.Text ?? "";
            string nimi = nimeCell?.Text ?? "Sõber";
            string msg = sõnumCell?.Text ?? "Tere!";
            if (string.IsNullOrWhiteSpace(email)) { await DisplayAlertAsync("Viga", "Sisesta emailiaadress!", "OK"); return; }
            var mail = new EmailMessage { Subject = $"Sõnum sõbrale {nimi}", Body = msg, BodyFormat = EmailBodyFormat.PlainText, To = new List<string> { email } };
            if (Email.Default.IsComposeSupported) await Email.Default.ComposeAsync(mail);
            else await DisplayAlertAsync("Viga", "Email pole toetatud", "OK");
        }

        // ---- Tervitus ----
        void ValiTervitus_Clicked(object? sender, EventArgs e)
        {
            valitudTervitus = tervitused[new Random().Next(tervitused.Count)];
            if (tervitusLabel != null) tervitusLabel.Text = $"✅ {valitudTervitus}";
        }

        async void SaadaTervitusSms_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus)) { await DisplayAlertAsync("Viga", "Vali esmalt tervitus!", "OK"); return; }
            string phone = telefonCell?.Text ?? "";
            if (string.IsNullOrWhiteSpace(phone)) { await DisplayAlertAsync("Viga", "Sisesta telefoninumber!", "OK"); return; }
            if (Sms.Default.IsComposeSupported) await Sms.Default.ComposeAsync(new SmsMessage(valitudTervitus, phone));
            else await DisplayAlertAsync("Viga", "SMS pole toetatud", "OK");
        }

        async void SaadaTervitusEmail_Clicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valitudTervitus)) { await DisplayAlertAsync("Viga", "Vali esmalt tervitus!", "OK"); return; }
            string email = emailCell?.Text ?? "";
            string nimi = nimeCell?.Text ?? "Sõber";
            if (string.IsNullOrWhiteSpace(email)) { await DisplayAlertAsync("Viga", "Sisesta emailiaadress!", "OK"); return; }
            var mail = new EmailMessage { Subject = $"🎉 Tervitus sõbrale {nimi}", Body = valitudTervitus, BodyFormat = EmailBodyFormat.PlainText, To = new List<string> { email } };
            if (Email.Default.IsComposeSupported) await Email.Default.ComposeAsync(mail);
            else await DisplayAlertAsync("Viga", "Email pole toetatud", "OK");
        }

        // ---- Вспомогательные ----
        Button TeeBtnNupp(string tekst, string värv, EventHandler handler)
        {
            var btn = new Button
            {
                Text = tekst,
                BackgroundColor = Color.FromArgb(värv),
                TextColor = Colors.White,
                CornerRadius = 20,
                FontSize = 13,
                Padding = new Thickness(12, 8)
            };
            btn.Clicked += handler;
            return btn;
        }
    }
}