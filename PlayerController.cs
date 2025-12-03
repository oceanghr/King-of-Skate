using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // VARIABEL PUBLIK (Diatur di Inspector - JANGAN LUPA!)
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float turnSpeed = 360f; // Kecepatan putar untuk gerakan halus

    // VARIABEL PENTING UNTUK LOMPAT
    public LayerMask groundLayer; // Harus diatur di Inspector (pilih layer Plane Anda)
    public float raycastLength = 0.2f; // Jarak tembak Raycast ke bawah

    // VARIABEL PRIVATE (Komponen Wajib)
    private Rigidbody rb;
    private Animator anim;

    void Start()
    {
        // Mendapatkan komponen. Harus ada Rigidbody dan Animator di objek yang sama!
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();

        // LOGIKA PENDARATAN: Mematikan animasi lompat saat menyentuh tanah.
        if (anim.GetBool("IsJumping") && IsGrounded())
        {
            anim.SetBool("IsJumping", false);
        }
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal"); // Input A/D
        float v = Input.GetAxis("Vertical");   // Input W/S

        Vector3 inputDirection = new Vector3(h, 0f, v).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // 1. ROTASI HALUS (Mengatur Arah Karakter)
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

            // Putar karakter secara bertahap (Smooth)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // 2. GERAKAN (Menerapkan Kecepatan)
            Vector3 moveVector = transform.forward * moveSpeed;
            // Terapkan kecepatan, sambil mempertahankan kecepatan vertikal (Y)
            rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
        }
        else
        {
            // Jika tidak ada input, hentikan gerakan horizontal
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        // SINKRONISASI ANIMASI LARI/DIAM
        float currentSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        anim.SetFloat("Speed", currentSpeed);
    }

    void Jump()
    {
        // Lompatan terjadi jika tombol Spasi ditekan DAN karakter berada di tanah
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                // SINKRONISASI ANIMASI LOMPAT
                anim.SetBool("IsJumping", true);
            }
        }
    }

    private bool IsGrounded()
    {
        // Menggunakan Raycast untuk mengecek Layer "Ground"
        bool hit = Physics.Raycast(transform.position, Vector3.down, raycastLength, groundLayer);

        // DEBUG VISUAL: Bantuan untuk melihat Raycast (Hijau=Sukses, Merah=Gagal)
        Color rayColor = (hit) ? Color.green : Color.red;
        Debug.DrawRay(transform.position, Vector3.down * raycastLength, rayColor);

        return hit;
    }
}
