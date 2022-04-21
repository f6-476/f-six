public interface IPowerUp {
    public void Attach(Ship ship);
}
public interface IEquipable : IPowerUp {}
public interface IThrowable : IPowerUp {}
