using Memory.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Memory.ViewModel
{
    public class BlockCollectionViewModel : ObservableObject
    {
        public ObservableCollection<BlockViewModel> Blocks { get; set; }
        private BlockViewModel? SelectedBlock1;
        private BlockViewModel? SelectedBlock2;

        private readonly DispatcherTimer _peekTimer;
        private readonly DispatcherTimer _openingTimer;

        private const int _peekSeconds = 3;
        private const int _openSeconds = 5;

        //constructor
        public BlockCollectionViewModel()
        {
            _peekTimer = new DispatcherTimer();
            _peekTimer.Interval = new TimeSpan(0, 0, _peekSeconds);
            _peekTimer.Tick += PeekTimer_Tick;

            _openingTimer = new DispatcherTimer();
            _openingTimer.Interval = new TimeSpan(0, 0, _openSeconds);
            _openingTimer.Tick += OpeningTimer_Tick;
        }

        public bool AreBlocksActive
        {
            get
            {
                if (SelectedBlock1 == null || SelectedBlock2 == null)
                    return true;
                return false;
            }
        }
        public bool AllBlockMatched
        {
            get
            {
                foreach (var block in Blocks)
                {
                    if (!block.IsMatched) return false;
                }
                return true;
            }
        }
        public bool CanSelect { get; set; }
        private List<Block> GetModelsFrom(string relativePath) //Hàm khởi tạo block model
        {
            var models = new List<Block>();
            var images = Directory.GetFiles(@relativePath, "*.jpg", SearchOption.AllDirectories);
            var id = 0;
            foreach (var image in images)
            {
                models.Add(new Block() { Id = id, ImageSource = "/Memory;component/" + image });
                id++;
            }
            return models;
        }
        private void ShuffleBlocks()
        {
            var rng = new Random();
            for (int i = 0; i < 64; i++)
            {
                Blocks.Reverse();
                Blocks.Move(rng.Next(0, Blocks.Count), rng.Next(0, Blocks.Count));
            }
        }
        public void CreateBlocks(string imagePath)
        {
            Blocks = new ObservableCollection<BlockViewModel>();
            var models = GetModelsFrom(@imagePath);

            for (int i = 0; i < 6; i++)
            {
                var newBlock = new BlockViewModel(models[i]);
                var newBlockMatched = new BlockViewModel(models[i]);
                Blocks.Add(newBlock);
                Blocks.Add(newBlockMatched);
                newBlock.PeekAtImage();
                newBlockMatched.PeekAtImage();
            }
            ShuffleBlocks();
            OnPropertyChanged(nameof(Blocks));
        }
        public void HideUnMacthed()
        {
            _peekTimer.Start();
        }
        public void SelectBlock(BlockViewModel block)
        {
            block.PeekAtImage();
            if(SelectedBlock1 == null) SelectedBlock1 = block;
            else if (SelectedBlock2 == null)
            {
                SelectedBlock2 = block;
                HideUnMacthed();
            }
            OnPropertyChanged(nameof(AreBlocksActive));
        }
        public void ClearSelected()
        {
            SelectedBlock1 = null;
            SelectedBlock2 = null;
            CanSelect = false;
        }
        public void MatchFailed()
        {
            SelectedBlock1.MarkFailed();
            SelectedBlock2.MarkFailed();
            ClearSelected();
        }
        public void MatchSuccess()
        {
            SelectedBlock1.MarkMatched();
            SelectedBlock2.MarkMatched();
            ClearSelected();
        }
        public bool CheckIfMatched()
        {
            if (SelectedBlock1.Id == SelectedBlock2.Id)
            {
                MatchSuccess();
                return true;
            }
            MatchFailed();
            return false;
        }
        public void RevealUnMatched()
        {
            foreach(var block in Blocks!)
            {
                if (!block.IsMatched)
                {
                    _peekTimer.Stop();
                    block.MarkFailed();
                    block.PeekAtImage();
                }
            }
        }
        public void Memorize()
        {
            _openingTimer.Start();
        }
        private void OpeningTimer_Tick(object sender, EventArgs e)
        {
            foreach(var block in Blocks!)
            {
                block.ClosePeek();
                CanSelect = true;
            }
            OnPropertyChanged(nameof(AreBlocksActive));
            _openingTimer.Stop();
        }
        private void PeekTimer_Tick(object sender, EventArgs e)
        {
            foreach (var block in Blocks!)
            {
                if (!block.IsMatched)
                {
                    block.ClosePeek();
                    CanSelect = true;
                }
            }
            OnPropertyChanged(nameof(AreBlocksActive));
            _peekTimer.Stop();
        }
    }
}
