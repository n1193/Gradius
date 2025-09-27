public class NormalBullet : Bullet
{
    // Start で speed を上書きしない！Initialize の override を殺すから
    protected override void Awake()
    {
        base.Awake();
        speed = 20f;
        // 既定速度はプレハブの serialized "speed" を使う。ここで固定値を入れない。
        // どうしても既定値をコードで持ちたいなら、speed が未設定(<=0)の時だけ入れる等にする
        // if (speed <= 0f) speed = 30f;
    }
}
