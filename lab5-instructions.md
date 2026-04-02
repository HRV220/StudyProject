## Шаг 2. Развёртывание в Kubernetes

Применить все манифесты:

```bash
kubectl apply -f redis-deployment.yml
kubectl apply -f redis-srv.yml
kubectl apply -f order-service-deployment.yml
kubectl apply -f order-service-srv.yml
kubectl apply -f notification-service-deployment.yml
```

Проверить, что все поды запустились:

```bash
kubectl get pods
```

Все поды должны быть в статусе `Running` и `READY 1/1`.

---

## Шаг 3. Проверка настроенных проб

### 3.1. Просмотр конфигурации проб

Убедиться, что пробы настроены в деплойментах:

```bash
kubectl describe deployment order-service-deployment
kubectl describe deployment notification-service-deployment
kubectl describe deployment redis-deployment
```

В выводе должны быть секции `Liveness` и `Readiness` с параметрами проб.

### 3.2. Проверка логов подов (пробы проходят успешно)

Посмотреть логи подов:

```bash
kubectl get pods
kubectl logs <имя-пода-order-service>
kubectl logs <имя-пода-notification-service>
```

Также можно посмотреть события подов:

```bash
kubectl describe pod <имя-пода-order-service>
```

В секции `Events` не должно быть ошибок, связанных с пробами.

### 3.3. Проверка работоспособности эндпоинта /health

```bash
kubectl port-forward service/order-service-service 5002:8080
```

В другом терминале:

```powershell
Invoke-WebRequest -Uri http://localhost:5002/health
```

Ответ: `Ok` (код 200).

---

## Шаг 4. Имитация неудачных проверок

### 4.1. Переключение флага destroy_flag

Отправить POST-запрос на `/switchFlag`, чтобы эндпоинт `/health` начал возвращать код 503:

```powershell
Invoke-WebRequest -Uri http://localhost:5002/switchFlag -Method POST
```

Ответ: `Flag:True` — теперь `/health` возвращает `503 Destroy`.

Убедиться:

```powershell
Invoke-WebRequest -Uri http://localhost:5002/health
```

Ответ: `Destroy` (код 503).

### 4.2. Наблюдение за реакцией Kubernetes

Подождать некоторое время (около 30 секунд) и проверить статус подов:

```bash
kubectl get pods -w
```

Флаг `-w` включает режим наблюдения в реальном времени. Должно быть видно, как Kubernetes:

1. Обнаруживает неудачную liveness-пробу
2. Перезапускает контейнер (колонка `RESTARTS` увеличивается)

### 4.3. Проверка логов и событий

Посмотреть события пода после срабатывания проб:

```bash
kubectl describe pod <имя-пода-order-service>
```

В секции `Events` должны появиться записи вида:

```
Warning  Unhealthy  Liveness probe failed: HTTP probe failed with statuscode: 503
Normal   Killing    Container order-service failed liveness probe, will be restarted
```

Посмотреть логи пода:

```bash
kubectl logs <имя-пода-order-service>
kubectl logs <имя-пода-order-service> --previous
```

Флаг `--previous` показывает логи предыдущего (перезапущенного) экземпляра контейнера.

---

## Шаг 5. Проверка Startup Probe (дополнительно)

Для проверки startup-пробы использовать альтернативный манифест:

```bash
kubectl apply -f notification-service-LS-deployment.yml
```

Этот манифест содержит `startupProbe`, которая выполняется первой. Liveness-проба начнёт работать только после успешного прохождения startup-пробы.

Проверить:

```bash
kubectl describe pod <имя-пода-notification-service>
```

---

## Шаг 6. Очистка

Удалить все ресурсы после завершения работы:

```bash
kubectl delete -f order-service-deployment.yml
kubectl delete -f order-service-srv.yml
kubectl delete -f notification-service-deployment.yml
kubectl delete -f redis-deployment.yml
kubectl delete -f redis-srv.yml
```

