using Memory.Model;
using System.Windows.Media;

namespace Memory.ViewModel
{
    public class BlockViewModel : ObservableObject
    {
        private Block _block;
        public int Id { get; set; }
        private bool _isViewed;
        private bool _isMatched;
        private bool _isFailed;

        public bool IsViewed
        {
            get { return _isViewed; }
            set
            {
                _isViewed = value;
                OnPropertyChanged(nameof(BlockImageStatus));
                OnPropertyChanged(nameof(BorderBrush));
            }
        }
        public bool IsMatched
        {
            get { return _isMatched; }
            set
            {
                _isMatched = value;
                OnPropertyChanged("BlockImageStatus");
                OnPropertyChanged("BorderBrush");
            }
        }
        public bool IsFailed
        {
            get { return _isFailed; }
            set
            {
                _isFailed = value;
                OnPropertyChanged(nameof(BlockImageStatus));
                OnPropertyChanged(nameof(BorderBrush));
            }
        }
        public bool IsSelectable
        {
            get
            {
                if (IsMatched) return false;
                if (IsViewed) return false;
                return true;
            }
        }
        public string BlockImageStatus //Trạng thái của block
        {
            get
            {
                if (_isMatched) return _block.ImageSource;
                if (_isViewed) return _block.ImageSource;
                return "/Memory;component/Assets/mystery_image.jpg";
            }
        }
        public Brush BorderBrush
        {
            get
            {
                if (IsFailed) return Brushes.Red;
                if (IsMatched) return Brushes.Green;
                if (IsViewed) return Brushes.Yellow;
                return Brushes.Black;
            }
        }

        //Constructor
        public BlockViewModel(Block block)
        {
            _block = block;
            Id = block.Id;
        }

        public void MarkMatched() => _isMatched = true;
        public void MarkFailed() => _isFailed = true;
        public void PeekAtImage()
        {
            IsViewed = true;
            OnPropertyChanged(nameof(BlockImageStatus));
        }
        public void ClosePeek()
        {
            IsViewed = false;
            IsFailed = false;
            OnPropertyChanged(nameof(IsSelectable));
            OnPropertyChanged(nameof(BlockImageStatus));
        }

    }
}
