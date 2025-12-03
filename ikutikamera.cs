using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ikutikamera : MonoBehaviour
{
    // Objek target yang akan diikuti (SkateBoard_V1)
    public Transform target;

    // Kehalusan gerakan kamera (Semakin kecil smoothSpeed, semakin lambat/halus)
    public float smoothSpeed = 0.125f;

    // Jarak relatif kamera dari target (X: samping, Y: tinggi, Z: belakang)
    // Coba Y=3, Z=-6 sebagai nilai awal
    public Vector3 offset = new Vector3(0f, 3f, -6f);

    // Kehalusan rotasi (Semakin besar rotationSpeed, semakin cepat kamera berputar)
    public float rotationSpeed = 5f;

    // Kami menggunakan LateUpdate agar kamera bergerak setelah target selesai bergerak
    void LateUpdate()
    {
        if (target == null) return;

        // 1. Hitung Posisi yang Diinginkan (Rotasi Target Diperhitungkan)
        // target.rotation * offset akan memutar offset sesuai rotasi target (penting untuk 360 derajat)
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // 2. Menggerakkan Kamera (Gerakan Halus menggunakan Lerp)
        // Time.deltaTime tidak diperlukan di sini karena smoothSpeed mengatur faktor lerp, bukan kecepatan konstan.
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. Mengarahkan Kamera (Rotasi Halus)
        // Kamera melihat ke arah target dengan smoothing (Slerp)
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);

        // Gunakan Slerp untuk rotasi yang halus
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}