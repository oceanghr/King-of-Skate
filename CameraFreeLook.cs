using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookBack : MonoBehaviour
{
    // --- PENGATURAN DARI IKUTIKAMERA ---
    [Header("Pengaturan Ikut Target")]
    public Transform target;
    public float followSmoothSpeed = 0.125f; // Kehalusan gerakan mengikuti posisi
    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public float defaultRotationSpeed = 5f; // Kecepatan rotasi kembali ke default

    // --- PENGATURAN DARI CAMERALOOKBACK ---
    [Header("Pengaturan Kontrol Bebas")]
    public float lookSensitivity = 5f; // Kecepatan putar saat klik kanan
    public float lookRotationSmoothing = 15f; // Kecepatan smoothing saat berputar/kembali

    // --- VARIABEL INTERNAL ---
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target untuk kamera belum diatur!");
            enabled = false;
            return;
        }

        // Kunci dan sembunyikan kursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inisialisasi rotasi akumulatif agar sesuai dengan rotasi kamera saat ini
        Vector3 currentEuler = transform.localEulerAngles;
        rotationX = (currentEuler.x > 180) ? currentEuler.x - 360 : currentEuler.x;
        rotationY = currentEuler.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. MENGATUR POSISI KAMERA (Selalu Aktif)
        Vector3 desiredPosition = target.position + target.rotation * offset;
        // Menggerakkan Kamera (Gerakan Halus menggunakan Lerp)
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothSpeed);

        // 2. MENGHITUNG ROTASI TARGET DEFAULT
        Quaternion defaultRotation = Quaternion.LookRotation(target.position - transform.position);

        // 3. KONTROL ROTASI BERDASARKAN INPUT MOUSE
        if (Input.GetMouseButton(1)) // KONDISI 1: Tombol Kanan Ditekan (Rotasi Bebas 360 Derajat)
        {
            // Ambil input mouse
            rotationY += Input.GetAxis("Mouse X") * lookSensitivity;
            rotationX -= Input.GetAxis("Mouse Y") * lookSensitivity;

            // Batasi rotasi vertikal (Pitch)
            rotationX = Mathf.Clamp(rotationX, -60f, 80f);

            // Hitung Rotasi Target Bebas
            Quaternion targetFreeRotation = Quaternion.Euler(rotationX, rotationY, 0f);

            // Terapkan Rotasi Bebas dengan Smoothing
            transform.rotation = Quaternion.Slerp(transform.rotation, targetFreeRotation, lookRotationSmoothing * Time.deltaTime);
        }
        else // KONDISI 2: Tombol Kanan Dilepas (Kembali ke Default Rotasi Mobil)
        {
            // Lakukan Smoothing untuk kembali ke rotasi default mobil
            transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, defaultRotationSpeed * Time.deltaTime);

            // Atur ulang nilai akumulatif rotasi saat hampir kembali, untuk mencegah 'lompatan'
            if (Quaternion.Angle(transform.rotation, defaultRotation) < 0.1f)
            {
                Vector3 defaultEuler = defaultRotation.eulerAngles;
                rotationX = (defaultEuler.x > 180) ? defaultEuler.x - 360 : defaultEuler.x;
                rotationY = defaultEuler.y;
            }
        }
    }
}