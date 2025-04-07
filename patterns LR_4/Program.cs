using System;
using System.Collections.Generic;

// Простір імен для поведінкових патернів
namespace Patterns.Behavioral
{
    // 1. Ланцюжок відповідальності (Chain of Responsibility)
    abstract class Handler
    {
        protected Handler next;
        public void SetNext(Handler next) => this.next = next;
        public abstract void Handle(string request);
    }

    class AuthHandler : Handler
    {
        public override void Handle(string request)
        {
            if (request == "auth")
                Console.WriteLine("AuthHandler обробив запит");
            else
                next?.Handle(request);
        }
    }

    class LogHandler : Handler
    {
        public override void Handle(string request)
        {
            if (request == "log")
                Console.WriteLine("LogHandler обробив запит");
            else
                next?.Handle(request);
        }
    }

    // 2. Команда (Command)
    interface ICommand { void Execute(); }

    class Light
    {
        public void On() => Console.WriteLine("Світло ввімкнено");
    }

    class LightOnCommand : ICommand
    {
        private Light light;
        public LightOnCommand(Light light) => this.light = light;
        public void Execute() => light.On();
    }

    // 3. Ітератор (Iterator)
    class NameRepository
    {
        private List<string> names = new() { "Анна", "Богдан", "Іван" };
        public IEnumerator<string> GetEnumerator() => names.GetEnumerator();
    }

    // 4. Посередник (Mediator)
    interface IChatMediator
    {
        void SendMessage(string message, User user);
        void AddUser(User user);
    }

    class Chat : IChatMediator
    {
        private List<User> users = new();
        public void AddUser(User user) => users.Add(user);

        public void SendMessage(string message, User sender)
        {
            foreach (var user in users)
            {
                if (user != sender)
                    user.Receive(message);
            }
        }
    }

    abstract class User
    {
        protected IChatMediator mediator;
        protected string name;

        public User(IChatMediator mediator, string name)
        {
            this.mediator = mediator;
            this.name = name;
        }

        public abstract void Send(string message);
        public abstract void Receive(string message);
    }

    class ChatUser : User
    {
        public ChatUser(IChatMediator mediator, string name) : base(mediator, name) { }

        public override void Send(string message)
        {
            Console.WriteLine($"{name} відправив: {message}");
            mediator.SendMessage(message, this);
        }

        public override void Receive(string message)
        {
            Console.WriteLine($"{name} отримав: {message}");
        }
    }

    // 5. Знімок (Memento)
    class Memento
    {
        public string State { get; }
        public Memento(string state) => State = state;
    }

    class Originator
    {
        public string State { get; set; }

        public Memento Save() => new(State);
        public void Restore(Memento memento) => State = memento.State;
    }

    // 6. Спостерігач (Observer)
    interface IObserver
    {
        void Update(string message);
    }

    interface ISubject
    {
        void Attach(IObserver observer);
        void Notify(string message);
    }

    class NewsAgency : ISubject
    {
        private List<IObserver> observers = new();
        public void Attach(IObserver observer) => observers.Add(observer);

        public void Notify(string message)
        {
            foreach (var observer in observers)
                observer.Update(message);
        }
    }

    class NewsReader : IObserver
    {
        private string name;
        public NewsReader(string name) => this.name = name;
        public void Update(string message) => Console.WriteLine($"{name} отримав новину: {message}");
    }

    // 7. Стан (State)
    interface IState { void Handle(); }

    class StartState : IState { public void Handle() => Console.WriteLine("Стан: СТАРТ"); }
    class StopState : IState { public void Handle() => Console.WriteLine("Стан: СТОП"); }

    class Context
    {
        private IState state;
        public void SetState(IState state)
        {
            this.state = state;
            state.Handle();
        }
    }

    // 8. Стратегія (Strategy)
    interface IStrategy { void Execute(); }

    class FastStrategy : IStrategy { public void Execute() => Console.WriteLine("Швидка стратегія"); }
    class SlowStrategy : IStrategy { public void Execute() => Console.WriteLine("Повільна стратегія"); }

    class StrategyContext
    {
        private IStrategy strategy;
        public void SetStrategy(IStrategy strategy) => this.strategy = strategy;
        public void Run() => strategy.Execute();
    }

    // 9. Шаблонний метод (Template Method)
    abstract class Game
    {
        public void Play()
        {
            Initialize();
            StartPlay();
            EndPlay();
        }

        protected abstract void Initialize();
        protected abstract void StartPlay();
        protected abstract void EndPlay();
    }

    class Football : Game
    {
        protected override void Initialize() => Console.WriteLine("Підготовка до футболу");
        protected override void StartPlay() => Console.WriteLine("Початок гри у футбол");
        protected override void EndPlay() => Console.WriteLine("Кінець матчу");
    }

    // 10. Відвідувач (Visitor)
    interface IElement { void Accept(IVisitor visitor); }

    class Book : IElement
    {
        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    class Pen : IElement
    {
        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    interface IVisitor
    {
        void Visit(Book book);
        void Visit(Pen pen);
    }

    class PriceVisitor : IVisitor
    {
        public void Visit(Book book) => Console.WriteLine("Ціна книги: 100 грн");
        public void Visit(Pen pen) => Console.WriteLine("Ціна ручки: 20 грн");
    }

    // === MAIN ===
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Chain of Responsibility ===");
            var auth = new AuthHandler();
            var log = new LogHandler();
            auth.SetNext(log);
            auth.Handle("log");

            Console.WriteLine("\n=== Command ===");
            var light = new Light();
            var command = new LightOnCommand(light);
            command.Execute();

            Console.WriteLine("\n=== Iterator ===");
            var repo = new NameRepository();
            foreach (var name in repo) Console.WriteLine(name);

            Console.WriteLine("\n=== Mediator ===");
            var chat = new Chat();
            var user1 = new ChatUser(chat, "Іван");
            var user2 = new ChatUser(chat, "Оля");
            chat.AddUser(user1);
            chat.AddUser(user2);
            user1.Send("Привіт!");

            Console.WriteLine("\n=== Memento ===");
            var origin = new Originator();
            origin.State = "Стан A";
            var saved = origin.Save();
            origin.State = "Стан B";
            origin.Restore(saved);
            Console.WriteLine("Поточний стан: " + origin.State);

            Console.WriteLine("\n=== Observer ===");
            var agency = new NewsAgency();
            var reader = new NewsReader("Іван");
            agency.Attach(reader);
            agency.Notify("Нова новина!");

            Console.WriteLine("\n=== State ===");
            var context = new Context();
            context.SetState(new StartState());
            context.SetState(new StopState());

            Console.WriteLine("\n=== Strategy ===");
            var stratContext = new StrategyContext();
            stratContext.SetStrategy(new FastStrategy());
            stratContext.Run();

            Console.WriteLine("\n=== Template Method ===");
            Game game = new Football();
            game.Play();

            Console.WriteLine("\n=== Visitor ===");
            var visitor = new PriceVisitor();
            IElement[] items = { new Book(), new Pen() };
            foreach (var item in items)
                item.Accept(visitor);
        }
    }
}
