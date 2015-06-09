volumes.AddFirst(helper.energies[(int)side]);
volumes.RemoveLast();

Vector3 position = Vector3.zero;

int i = 0;
foreach (float vol in volumes)
{
    float rate = i / (resolution - 1f);
    position.x = Mathf.Cos(rate * TWOPI) * radius;
    position.y = vol * amplitude;
    position.z = Mathf.Sin(rate * TWOPI) * radius;

    //points[i] = position;
    points[i] = Vector3.Lerp(points[i], position, Time.deltaTime * speed);
    lineRenderer.SetPosition(i, points[i]);

    i++;
}

lineRenderer.SetPosition(0, points[resolution - 1]);