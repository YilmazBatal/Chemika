using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    // Kutu Oranını 8:11 (1:1.375) kullanıyoruz. Genişlik = 8, Yükseklik = 11
    [Header("Kutu Boyutları (Oyun Alanı)")]
    [SerializeField] private float boxHeight = 11f; // Kutu yüksekliği
    [SerializeField] private float boxWidth = 8.2f; // Kutu genişliği (Oran: 8/11 = ~0.727)

    [Header("Referanslar")]
    [SerializeField] private Transform bottomWall; // Kutunun alt duvarının Transform bileşeni

    void Start()
    {
        AdjustCamera();
    }

    void Update()
    {
        // Geliştirme aşamasında değişiklikleri görmek için Update'te çağırın
        AdjustCamera();
    }

    void AdjustCamera()
    {
        Camera cam = Camera.main;
        if (cam == null || bottomWall == null) return;

        float targetAspect = boxWidth / boxHeight; // Hedef oran: 8 / 11 = ~0.727
        float windowAspect = (float)Screen.width / Screen.height;

        // 1. Orthographic Size (Kamera Büyüklüğünü) Ayarlama:
        // * Kutu genişliğine göre kamerayı ayarlama
        // * Amaç: Kutuyu ekranın genişliğine göre orantılı olarak doldurmak.
        float desiredOrthographicSize = (boxWidth / 2f) / windowAspect;
        cam.orthographicSize = desiredOrthographicSize;

        // 2. Kutuyu Ekranın Altına Yapıştırma:
        // Kamera Y pozisyonunu ayarlayarak kutu tabanını ekranın en altına itiyoruz.

        // Kameranın orthographicSize'ı, ekranın yarım yüksekliğini (world units) temsil eder.
        float halfScreenHeightWorld = cam.orthographicSize;

        // Kamera pozisyonunun, dünya koordinatlarında kutu tabanına göre ne kadar yukarıda olması gerektiğini hesaplıyoruz.
        // bottomWall'ın Y pozisyonu (diyelim ki 0) + (Ekran Yüksekliği / 2) - (Kutu Yüksekliği / 2)
        
        // bottomWall'ın tam olarak ekranın alt kenarında görünmesi için gereken Y pozisyonu:
        float newCameraY = bottomWall.position.y + halfScreenHeightWorld - (boxHeight / 2f);

        // Kameranın yeni pozisyonunu ayarla
        cam.transform.position = new Vector3(
            cam.transform.position.x,
            newCameraY,
            cam.transform.position.z // Z değeri sabit kalmalı
        );
    }
}