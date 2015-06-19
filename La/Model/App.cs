using LibNode;

namespace La.Model
{
    public interface IApp
    {
        IEntityRepo EntityRepo { get; }
        ISym Sym { get; set; }
    }

    public class App : IApp
    {
        public App()
        {
            EntityRepo = new EntityRepoMem();
        }

        public IEntityRepo EntityRepo { get; }

        private ISym _sym;
        public ISym Sym
        {
            get { return _sym; }
            set { _sym = value; }
        }
    }
}
