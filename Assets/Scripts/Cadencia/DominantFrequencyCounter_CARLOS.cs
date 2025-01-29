using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class DominantFrequencyCounter_CARLOS : MonoBehaviour
{
    //Datos reales
    public List<float> data;
    [SerializeField] string fileName;

    public float[] inputData; // Datos de la posición en x respecto al tiempo
    float sampleRate; // Frecuencia de muestreo en segundos
    int maxFFTIndex; // El índice en la FFT donde está el pico (la frecuencia)

    float frequency;
    float totalCadence;

    void Start()
    {
        ReadCSVFile(fileName);

        //Eliminar hasta el dato 300
        data = data.GetRange(300, data.Count - 300);

        string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + fileName + "_FFT.csv";
        Debug.Log($"Ruta: {filePath}");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Nombre;Intervalo 512;FFT;Cadencia");

            // Procesar los datos en intervalos dinámicos de 128 frames
            for (int i = 0; i < data.Count - 512; i += 32)
            {
                // Obtener el bloque actual de datos de 128 frames
                List<float> block = data.GetRange(i, 512);

                // Calcular los índices del intervalo
                int startInterval = 300 + i;
                int endInterval = startInterval + 512;

                // Imprimir el intervalo
                Debug.Log($"Intervalo: {startInterval}-{endInterval}");

                if (endInterval == 1580)
                {
                    Debug.Log("FIN");

                    AdjustListLengthToPowerOfTwo(data);
                    inputData = data.ToArray();
                    DoFFT(inputData);
                    totalCadence = frequency * 60f;
                    Debug.Log("----FRECUENCIA TOTAL-----");
                    Debug.Log("[FFT] Frecuencia dominante: " + frequency);
                    Debug.Log("[FFT] Cadencia: " + totalCadence);
                    writer.WriteLine($"Cadencia total;1024(300-1324);{frequency};{totalCadence}");
                    break;
                }
                else
                {
                    inputData = block.ToArray();
                    DoFFT(inputData);
                    Debug.Log("----FRECUENCIA MEDIA DATOS PRUEBA: METODO TRANSFORMADA RÁPIDA DE FOURIER-----");
                    Debug.Log("[FFT] Frecuencia dominante: " + frequency);
                    Debug.Log("[FFT] Cadencia: " + frequency * 60f);
                    writer.WriteLine($"{fileName};{startInterval} - {endInterval};{frequency};{frequency * 60f}");
                }
            }
        }
    }

    bool IsPowerOfTwo(int n)
    {
        return (n > 0) && (n & (n - 1)) == 0;
    }

    void AdjustListLengthToPowerOfTwo(List<float> list)
    {
        int originalLength = list.Count;

        if (IsPowerOfTwo(originalLength))
        {
            Debug.Log("La longitud de la lista ya es una potencia de 2.");
            return;
        }

        int newLength = 1;
        while (newLength * 2 <= originalLength)
        {
            newLength *= 2;
        }

        list.RemoveRange(newLength, list.Count - newLength);
        Debug.Log("Lista recortada a la longitud más cercana que es potencia de 2: " + newLength);
    }

    void ReadCSVFile(string fileName)
    {
        data = new List<float>();
        TextAsset csvData = Resources.Load<TextAsset>(fileName);

        if (csvData == null)
        {
            Debug.LogError("Archivo CSV no encontrado.");
            return;
        }

        using (StringReader reader = new StringReader(csvData.text))
        {
            bool isFirstLine = true;
            int yColumnIndex = -1;

            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                if (isFirstLine)
                {
                    string[] headers = line.Split(';');
                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (headers[i].Trim() == "Posicion Y")
                        {
                            yColumnIndex = i;
                            break;
                        }
                    }
                    isFirstLine = false;

                    if (yColumnIndex == -1)
                    {
                        Debug.LogError("No se encontró la columna 'Posicion Y' en el archivo CSV.");
                        return;
                    }
                }
                else
                {
                    string[] fields = line.Split(';');
                    if (fields.Length > yColumnIndex)
                    {
                        if (float.TryParse(fields[yColumnIndex], out float yPos))
                        {
                            data.Add(yPos);
                        }
                        else
                        {
                            Debug.LogWarning("No se pudo convertir el valor de 'Posicion Y' a float en la línea: " + line);
                        }
                    }
                }
            }
        }

        Debug.Log("Se han cargado " + data.Count + " posiciones Y.");
    }

    public float DoFFT(float[] FFTdata)
    {
        Complex[] complexData = new Complex[FFTdata.Length];
        for (int i = 0; i < FFTdata.Length; i++)
        {
            complexData[i] = new Complex(FFTdata[i], 0);
        }
        FFT(complexData);

        sampleRate = FFTdata.Length / 30;

        float maxMagnitude = 0;
        for (int i = 5; i < complexData.Length / 2; i++)
        {
            float magnitude = complexData[i].Magnitude;
            if (magnitude > maxMagnitude)
            {
                maxMagnitude = magnitude;
                maxFFTIndex = i;
            }
        }

        frequency = maxFFTIndex / sampleRate;
        return frequency;
    }

    public void FFT(Complex[] data)
    {
        int n = data.Length;
        int m = (int)Mathf.Log(n, 2);

        for (int i = 0; i < n; i++)
        {
            int j = ReverseBits(i, m);
            if (j > i)
            {
                Complex temp = data[i];
                data[i] = data[j];
                data[j] = temp;
            }
        }

        for (int i = 1; i <= m; i++)
        {
            int size = 1 << i;
            int halfSize = size / 2;
            Complex twiddleFactorStep = new Complex(Mathf.Cos(Mathf.PI / halfSize), Mathf.Sin(Mathf.PI / halfSize));

            Complex twiddleFactor = new Complex(1, 0);
            for (int j = 0; j < halfSize; j++)
            {
                for (int k = j; k < n; k += size)
                {
                    int rightIndex = k + halfSize;
                    Complex temp = data[rightIndex] * twiddleFactor;
                    data[rightIndex] = data[k] - temp;
                    data[k] += temp;
                }
                twiddleFactor *= twiddleFactorStep;
            }
        }
    }

    int ReverseBits(int val, int width)
    {
        int result = 0;
        for (int i = 0; i < width; i++)
        {
            result <<= 1;
            result |= val & 1;
            val >>= 1;
        }
        return result;
    }

    public class Complex
    {
        public float Real { get; set; }
        public float Imaginary { get; set; }

        public Complex(float real, float imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public float Magnitude
        {
            get { return Mathf.Sqrt(Real * Real + Imaginary * Imaginary); }
        }

        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Real - b.Real, a.Imaginary - b.Imaginary);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Real * b.Real - a.Imaginary * b.Imaginary,
                               a.Real * b.Imaginary + a.Imaginary * b.Real);
        }
    }
}
