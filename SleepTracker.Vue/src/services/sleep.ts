export interface SleepReadDto {
  id: number;
  start: string;
  end: string;
  durationHours: string;
}

export interface SleepCreateDto {
  start: string;
  end: string;
}

export interface SleepUpdateDto {
  start?: string;
  end?: string;
}

export interface PagedResponse<T> {
  status: number;
  message?: string;
  data?: T;
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
}

const API_URL = import.meta.env.VITE_API_URL;

const baseUrl = `${API_URL}/sleeps`;

export async function getSleeps(
  pageNumber: number = 1,
  pageSize: number = 10,
  startDate?: Date,
  endDate?: Date
): Promise<PagedResponse<SleepReadDto[]>> {
  const params = new URLSearchParams();
  params.append('Page', pageNumber.toString());
  params.append('PageSize', pageSize.toString());

  if (startDate) {
    const normalizedStart = new Date(startDate);
    normalizedStart.setHours(0, 0, 0, 0);
    params.append('Start', normalizedStart.toISOString());
  }

  if (endDate) {
    const normalizedEnd = new Date(endDate);
    normalizedEnd.setHours(23, 59, 59, 999);
    params.append('End', normalizedEnd.toISOString());
  }

  const response = await fetch(`${baseUrl}?${params.toString()}`);

  const data = await response.json();

  return data;
}

export async function createSleep(
  createDto: SleepCreateDto
): Promise<SleepReadDto> {
  const response = await fetch(baseUrl, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(createDto),
  });

  const data = await response.json();
  return data;
}

export async function updateSleep(
  id: number,
  updateDto: SleepUpdateDto
): Promise<void> {
  const response = await fetch(`${baseUrl}/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(updateDto),
  });

  if (!response.ok) {
    throw new Error('Failed to update sleep record');
  }
}

export async function deleteSleep(id: number): Promise<void> {
  const response = await fetch(`${baseUrl}/${id}`, {
    method: 'DELETE',
  });

  if (!response.ok) {
    throw new Error('Failed to delete sleep record');
  }
}

