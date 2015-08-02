﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ScorePredict.Common.Data;
using ScorePredict.Common.Utility;
using ScorePredict.Core.Contracts;
using ScorePredict.Core.MessageBus;
using ScorePredict.Core.MessageBus.Messages;
using ScorePredict.Core.Pages;
using ScorePredict.Core.ViewModels.Abstract;
using ScorePredict.Services;
using ScorePredict.Services.Contracts;
using Xamarin.Forms;
using System;

namespace ScorePredict.Core.ViewModels
{
    public class PredictionsPageViewModel : ScorePredictRootPageViewModel
    {
        public IPredictionService PredictionService { get; private set; }
        public IBus MessageBus { get; private set; }
        public IReadUserSecurityService ReadUserSecurityService { get; private set; }

        private ObservableCollection<PredictionGroup> _predictionGroups;
        public ObservableCollection<PredictionGroup> PredictionGroups
        {
            get { return _predictionGroups; }
            set
            {
                _predictionGroups = value;
                OnPropertyChanged();
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SelectPredictionCommand
        {
            get
            {
                return new Command<Prediction>(
                    x => Navigation.PushModalAsync(new ScorePredictNavigationPage(new PredictionEditPage(x))));
            }
        }

        public ICommand RefreshCommand { get { return new Command(Refresh); } }

        public PredictionsPageViewModel(IPredictionService predictionService, IDialogService dialogService,
            IClearUserSecurityService clearUserSecurityService, IBus messageBus, IReadUserSecurityService readUserSecurityService,
            IKillApplication killApp)
            : base(clearUserSecurityService, dialogService, killApp)
        {
            PredictionService = predictionService;
            MessageBus = messageBus;
            ReadUserSecurityService = readUserSecurityService;
        }

        public override async void OnShow()
        {
            if (IsLoaded) return;

            if (ReadUserSecurityService.ReadUser() != null)
            {
                try
                {
                    IsBusy = true;
                    LoaderMessage = "Loading Predictions...";
                    await LoadPredictionsAsync();

                    IsLoaded = true;
                }
                catch (Exception)
                {
                    DialogService.Alert("Failed to load Predictions. Please refresh");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async void Refresh()
        {
            try
            {
                DialogService.ShowLoading("Refreshing...");
                IsBusy = true;
                await LoadPredictionsAsync();
            }
            catch (Exception)
            {
                DialogService.Alert("Failed to Refresh Predictions. Please try again");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadPredictionsAsync()
        {
            var result = await PredictionService.GetCurrentWeekPredictions();
            PredictionGroups = new ObservableCollection<PredictionGroup>((new List<PredictionGroup>
                {
                    new PredictionGroup("Pregame", result.Where(x => x.InPregame).ToList()),
                    new PredictionGroup("Final", result.Where(x => x.IsConcluded).ToList()),
                    new PredictionGroup("In Progress", result.Where(x => !x.IsConcluded && !x.InPregame).ToList())
                }).Where(pg => pg.Count > 0));
        }
    }
}
